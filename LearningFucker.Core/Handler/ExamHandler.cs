using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LearningFucker.Models;

namespace LearningFucker.Handler
{
    public class ExamHandler : TaskHandlerBase
    {
        private Models.CourseList courseList;
        private Service.StudyService studyService;

        public ExamHandler(Models.CourseList courseList, CancellationToken token, Models.Task task)
            :base(token, task)
        {
            this.courseList = courseList;
        }

        public async override System.Threading.Tasks.Task DoWork()
        {
            try
            {
                decimal integral = 0;
                while (true)
                {
                    if (CancellationToken.IsCancellationRequested)
                        break;
                    Random random = new Random();
                    int id = random.Next(0, courseList.List.Count);

                    var course = courseList.List[id];
                    
                    integral += await studyService.Exam(course);
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
            if (courseList.List[0].Appendix == null)
                throw new Exception("获取课程附加信息时出错, 请重新开启程序!");

            this.LimitIntegral = courseList.List[0].ExamMaxIntegral;
            return true;
        }

    }
}
