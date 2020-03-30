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

        public async override void DoWork()
        {
            Random random = new Random();
            int id = random.Next(0, courseList.List.Count - 1);

            var course = courseList.List[id];

            var examList = await Fucker.GetExamList(course);

            if(examList == null || examList.Count == 0)
            {
                DoWork();
                return;
            }

            DoExam(course, examList.List[0], examList);
        }

        public async void DoExam( Course course, Exam exam, ExamList examList)
        {
            await Fucker.GetExamDetail(exam);
            if(exam.ExamDetail == null || exam.ExamDetail.AllowExam == false)
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

            }
        }

        public async Task<decimal> StartExam(Exam exam)
        {
            DataContext dataContext = new DataContext();
            bool invalid = false;
            List<Answer> answers = new List<Answer>();
            while(true)
            {
                if (!await Fucker.StartExam(exam))
                    return 0;

                if (exam.Papers == null || exam.Papers.Count == 0)
                    return 0;

                var paper = exam.Papers[exam.Papers.Count - 1];

                foreach (var item in paper.Questions)
                {
                    var question = dataContext.GetRow(item.TmID);
                    if(question == null)
                    {
                        invalid = true;
                        dataContext.InsertRow(item);

                    }
                    else
                    {
                        if(!invalid)
                        {
                            Answer answer = new Answer();
                            answer.TmID = item.TmID;
                            answer.AnswerContent = question.Answers.Replace(";", ",");
                            answers.Add(answer);
                        }
                    }
                }

                if (!await Fucker.HandIn(paper, exam, answers))
                    continue;

                if (await Fucker.GetResult(paper.Result))
                {
                    if(paper.Result.Integral == 0)      //答题失败
                    {
                        if(await Fucker.ReviewPaper(paper.Result, exam))
                        {
                            foreach(var item in paper.Result.Questions)
                            {
                                if(dataContext.GetRow(item.TmID) == null)
                                {
                                    dataContext.InsertRow(item);
                                }
                            }
                        }
                        continue;
                    }
                    else
                    {
                        return paper.Result.Integral;
                    }
                }
                else
                {
                    continue;
                }                

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

        public override bool Stop()
        {
            throw new NotImplementedException();
        }

        protected override bool Complete()
        {
            throw new NotImplementedException();
        }
    }
}
