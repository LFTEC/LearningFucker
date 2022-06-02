﻿using System;
using System.Collections.Generic;
using System.Text;
using LearningFucker.Models;
using System.Threading;

namespace LearningFucker.Service
{
    public class StudyService
    {
        public StudyService(Fucker fucker, CourseService courseService, CancellationToken token)
        {
            this.fucker = fucker;
            this.courseService = courseService;
            this.token = token;
        }

        private Fucker fucker;
        private CourseService courseService;
        public Study Current { get; private set; }
        private CancellationToken token;

        public async System.Threading.Tasks.Task Start(Course course)
        {
            try
            {
                if (Convert.ToDateTime(course.ValidateTimeB) > DateTime.Today || Convert.ToDateTime(course.ValidateTimeE) < DateTime.Today)
                {
                    return;
                }

                await courseService.GetCourseDetail(course);
                await courseService.GetCourseAppendix(course);

                foreach (var item in course.Detail.WareList)
                {
                    if (item.AllowIntegral == 0)
                        continue;
                    if (token.IsCancellationRequested)
                        break;
                    await courseService.GetIntegralInfo(course);
                    if (course.SumIntegral >= course.MaxIntegral)
                        break;

                    var study = await fucker.StartStudy(course, item);
                    Current = study;
                    await study.Start(fucker, token); 
                }
            }
            catch(Exception ex)
            {

            }
        }

        public async System.Threading.Tasks.Task Start(ElectiveCourse course)
        {
            try
            {
                

                await courseService.GetCourseDetail(course);
                await courseService.GetCourseAppendix(course);
                

                foreach (var item in course.Detail.WareList)
                {
                    if (item.AllowIntegral == 0)
                        continue;
                    if (token.IsCancellationRequested)
                        break;
                    await courseService.GetIntegralInfo(course);
                    if (course.SumIntegral >= course.MaxIntegral)
                        break;
                    var study = await fucker.StartStudy(course, item);
                    Current = study;
                    await study.Start(fucker, token);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public async System.Threading.Tasks.Task<decimal> Exam(Course course)
        {
            try
            {
                decimal integral = 0;
                await courseService.GetCourseAppendix(course);
                if (course?.Appendix.IsExam == false)
                    return 0;
                var examList = await courseService.GetExamListAsync(course);
                foreach (var exam in examList?.List)
                {
                    await courseService.GetExamDetailAsync(exam);
                    if (exam?.ExamDetail.AllowExam == true)
                    {
                        integral += await StartExam(exam);
                    }
                }
                return integral;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async System.Threading.Tasks.Task<decimal> Exercise(ElectiveCourse course)
        {
            try
            {
                await fucker.GetExerciseAllowIntegral(course);
                if (course.AllowExerciseIntegral <= 0)
                {
                    return 0;
                }

                List<ExerciseAnswer> answers = new List<ExerciseAnswer>();

                if (!await fucker.StartExercise(course))
                    return 0;

                if (course.Exercises == null || course.Exercises.Count == 0)
                    return 0;

                var paper = course.Exercises[course.Exercises.Count - 1];
                answers.Clear();
                foreach (var item in paper.Questions)
                {
                    ExerciseAnswer answer = new ExerciseAnswer();
                    answer.TmID = item.TmID;
                    answers.Add(answer);

                    answer.AnswerContent = item.Answers.Replace(";", ",");
                }

                if (!await fucker.HandIn(course, paper, answers, 5))
                    return 0;

                await UpdateQuestionBank(paper.Result);

                if (await fucker.GetResult(paper.Result))
                {
                    return paper.Result.Integral;
                }
                else
                {
                    return 0;
                }

                
            }
            catch (Exception ex)
            {
                fucker.Worker.ReportError(ex.Message);
                return 0;
            }
        }

        private async System.Threading.Tasks.Task<decimal> StartExam(Exam exam)
        {
            try
            {
                DataContext dataContext = new DataContext();
                List<Answer> answers = new List<Answer>();
                int allowCount = (int)(exam.ExamDetail.MaxExamCount * 0.8);
                int testedCount = exam.ExamDetail.JoinCount;
                while (true)
                {
                    testedCount++;
                    if (testedCount > allowCount)
                        throw new Exception($"用户{fucker.Worker.User.RealName}的考试{exam.ExamName}已达次数上限且仍未通过，请手动答题。");

                    if (!await fucker.StartExam(exam))
                        return 0;

                    if (exam.Papers?.Count == 0)
                        return 0;

                    var paper = exam.Papers[exam.Papers.Count - 1];
                    answers.Clear();
                    foreach (var item in paper.Questions)
                    {
                        var question = await dataContext.GetRow(item.TmID);
                        Answer answer = new Answer();
                        answer.TmID = item.TmID;
                        answers.Add(answer);

                        if (question == null)
                        {
                            answer.AnswerContent = "";
                        }
                        else
                        {
                            answer.AnswerContent = question.Answers.Replace(";", ",");
                        }
                        await System.Threading.Tasks.Task.Delay(1000);
                    }

                    if (!await fucker.HandIn(paper, exam, answers))
                        continue;

                    await fucker.GetResult(paper.Result);


                    if (paper.Result.ErrorQuestionCount != 0)      //答题失败
                    {
                        await UpdateQuestionBank(paper.Result, exam);
                        continue;
                    }
                    else
                    {
                        return paper.Result.Integral;
                    }

                }
            }
            catch (Exception ex)
            {
                fucker.Worker.ReportError(ex.Message);
                return 0;
            }
        }

        private async System.Threading.Tasks.Task UpdateQuestionBank(Result result, Exam exam)
        {
            try
            {
                DataContext dataContext = new DataContext();
                if (await fucker.ReviewPaper(result, exam))
                {
                    foreach (var item in result.Questions)
                    {
                        var row = await dataContext.GetRow(item.TmID);
                        if (row == null)
                        {
                            await dataContext.InsertRow(item);
                        }
                        else if (row.Answers != item.Answers)
                        {
                            await dataContext.UpdateRow(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                fucker.Worker.ReportError(ex.Message);
            }
        }

        private async System.Threading.Tasks.Task UpdateQuestionBank(ExerciseResult result)
        {
            try
            {
                DataContext dataContext = new DataContext();
                if (await fucker.ReviewExercise(result))
                {
                    foreach (var item in result.Questions)
                    {
                        var row = await dataContext.GetRow(item.TmID);
                        if (row == null)
                        {
                            await dataContext.InsertRow(item);
                        }
                        else if (row.Answers != item.Answers)
                        {
                            await dataContext.UpdateRow(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                fucker.Worker.ReportError(ex.Message);
            }
        }

        public async System.Threading.Tasks.Task<Study> GetStudyLogAsync()
        {
            try
            {
                await Current.GetStudyInfo();
                return Current;
            }   
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
