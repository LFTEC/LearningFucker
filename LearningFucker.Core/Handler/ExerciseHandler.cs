using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using LearningFucker.Models;

namespace LearningFucker.Handler
{
    public class ExerciseHandler : TaskHandlerBase
    {
        public ExerciseHandler(ElectiveCourseList courseList, CancellationToken token, Models.Task task)
            :base(token, task)
        {
            this.courseList = courseList;
            cancellationTokenSource = new CancellationTokenSource();
        }
        private Models.ElectiveCourseList courseList;
        private Service.StudyService studyService;
        private CancellationTokenSource cancellationTokenSource;

        public async override System.Threading.Tasks.Task DoWork()
        {
            try
            {
                while (true)
                {
                    if (CancellationToken.IsCancellationRequested)
                        break;

                    Random random = new Random();
                    int id = random.Next(0, courseList.List.Count);
                    var course = courseList.List[id];
                    await new Service.CourseService(Fucker).GetCourseDetail(course);
                    if (course.Detail.QuestionCount <= 0)//无练习题的选修课, 退出重选
                    {
                        await System.Threading.Tasks.Task.Delay(100);
                        continue;
                    }

                    await studyService.Exercise(course);
                    await new Service.CourseService(Fucker).GetIntegralInfo(course);
                    if (course.ExamIntegral >= this.LimitIntegral)
                    {
                        Complete();
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Stop();
            }
        }


        protected override async Task<bool> Start()
        {
            var courseService = new Service.CourseService(Fucker);
            studyService = new Service.StudyService(Fucker, courseService, CancellationToken);
            await Fucker.GetCourseAppendix(courseList.List[0]);
            await courseService.GetIntegralInfo(courseList.List[0]);
            this.LimitIntegral = courseList.List[0].ExamMaxIntegral;
            return true;
        }


    }
}
