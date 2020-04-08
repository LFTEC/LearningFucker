using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningFucker.Models;

namespace LearningFucker.Handler
{
    public class ExamHandler : TaskHandlerBase
    {
        private Models.CourseList courseList;
        private Exam Exam { get; set; }

        public async override void DoWork()
        {
            try
            {
                Random random = new Random();
                int id = random.Next(0, courseList.List.Count - 1);

                var course = courseList.List[id];

                var examList = await Fucker.GetExamList(course);

                if (examList == null || examList.Count == 0)
                {
                    DoWork();
                    return;
                }

                DoExam(course, examList.List[0], examList);
            }
            catch(Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Stop();
            }
        }

        public async void DoExam( Course course, Exam exam, ExamList examList)
        {
            try
            {
                if (this.TaskStatus == TaskStatus.Stopping)
                {
                    TaskStatus = TaskStatus.Stopped;
                    return;
                }

                this.Exam = exam;
                await Fucker.GetExamDetail(exam);
                if (exam.ExamDetail == null || exam.ExamDetail.AllowExam == false)
                {
                    var index = examList.List.IndexOf(exam);
                    if (index == examList.Count - 1)
                        DoWork();
                    else
                    {
                        index++;
                        DoExam(course, examList.List[index], examList);
                    }
                }
                else
                {
                    var integral = await StartExam(exam);
                    TaskForWork.Integral += integral;
                    if (TaskForWork.LimitIntegral <= TaskForWork.Integral)
                    {
                        this.Complete();
                    }
                    else
                    {
                        var index = examList.List.IndexOf(exam);
                        if (index == examList.Count - 1)
                            DoWork();
                        else
                        {
                            index++;
                            DoExam(course, examList.List[index], examList);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Stop();
            }
        }

        public async Task<decimal> StartExam(Exam exam)
        {
            try
            {
                DataContext dataContext = new DataContext();
                List<Answer> answers = new List<Answer>();
                while (true)
                {
                    if (!await Fucker.StartExam(exam))
                        return 0;

                    if (exam.Papers == null || exam.Papers.Count == 0)
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
                    }

                    if (!await Fucker.HandIn(paper, exam, answers))
                        continue;

                    await Fucker.GetResult(paper.Result);


                    if (paper.Result.ErrorQuestionCount != 0)      //答题失败
                    {
                        await UpdateQuestionBank(paper.Result);
                        continue;
                    }
                    else
                    {
                        return paper.Result.Integral;
                    }

                }
            }
            catch(Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Stop();
                return 0;
            }
        }

        private async System.Threading.Tasks.Task UpdateQuestionBank(Result result)
        {
            try
            {
                DataContext dataContext = new DataContext();
                if (await Fucker.ReviewPaper(result, Exam))
                {
                    foreach (var item in result.Questions)
                    {
                        if (await dataContext.GetRow(item.TmID) == null)
                        {
                            await dataContext.InsertRow(item);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Stop();
            }
        }

        public override bool Start(Fucker fucker)
        {
            if (!base.Start(fucker)) return false;

            Start();
            return true;
        }

        public async void Start()
        {
            try
            {
                var courseList = await Fucker.GetCourseList(0, 100);
                if (courseList == null || courseList.List == null || courseList.List.Count == 0)
                    throw new Exception("not implemented");

                this.courseList = courseList;

                await Fucker.GetCourseAppendix(courseList.List[0]);
                if (courseList.List[0].Appendix == null)
                    throw new Exception("获取课程附加信息时出错, 请重新开启程序!");

                this.TaskForWork.LimitIntegral = courseList.List[0].Appendix.MaxExamIntegral;

                DoWork();
            }
            catch(Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Stop();
            }
        }

    }
}
