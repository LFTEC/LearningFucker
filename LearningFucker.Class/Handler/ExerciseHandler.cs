using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningFucker.Models;

namespace LearningFucker.Handler
{
    public class ExerciseHandler : TaskHandlerBase
    {
        private Models.ElectiveCourseList courseList;

        public async override void DoWork()
        {
            try
            {
                if (this.TaskStatus == TaskStatus.Stopping)
                {
                    TaskStatus = TaskStatus.Stopped;
                    return;
                }

                Random random = new Random();
                int id = random.Next(0, courseList.List.Count - 1);

                var course = courseList.List[id];

                var integral = await StartExercise(course);
                TaskForWork.Integral += integral;
                if (TaskForWork.LimitIntegral <= TaskForWork.Integral)
                {
                    this.Complete();
                }
                else
                {
                    DoWork();
                }
            }
            catch(Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Stop();
            }
        }


        public async Task<decimal> StartExercise(ElectiveCourse course)
        {
            try
            {
                List<ExerciseAnswer> answers = new List<ExerciseAnswer>();

                if (!await Fucker.StartExercise(course))
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

                if (!await Fucker.HandIn(course, paper, answers, 5))
                    return 0;

                if (await Fucker.GetResult(paper.Result))
                {
                    return paper.Result.Integral;
                }
                else
                {
                    return 0;
                }
            }
            catch(Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                return 0;
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
                var propertyList = await Fucker.GetPropertyList();

                this.courseList = new ElectiveCourseList();
                this.courseList.List = new List<ElectiveCourse>();

                propertyList.List.ForEach(s =>
                {
                    s.SubNodes.ForEach(async n =>
                    {
                        var courselist = await Fucker.GetElectiveCourseList(n);
                        this.courseList.Count += courselist.Count;
                        this.courseList.List.AddRange(courselist.List);

                    });
                });

                var list = await Fucker.GetElectiveCourseList(propertyList.List[0].SubNodes[0]);

                await Fucker.GetCourseAppendix(list.List[0]);

                this.TaskForWork.LimitIntegral = list.List[0].Appendix.MaxExamIntegral;
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
