using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningFucker.Models;
using System.Diagnostics;
using System.Threading;

namespace LearningFucker.Handler
{
    public class ElectiveHandler : TaskHandlerBase
    {
        public ElectiveHandler(ElectiveCourseList courseList, CancellationToken token, Models.Task task)
            :base(token, task)
        {
            this.courseList = courseList;
            cancel = new CancellationTokenSource();
        }


        private Models.ElectiveCourseList courseList;
        private System.Timers.Timer timer;
        private Service.StudyService studyService;
        private CancellationTokenSource cancel;


        public async override System.Threading.Tasks.Task DoWork()
        {
            try
            {
                while(true)
                {
                    if (CancellationToken.IsCancellationRequested)
                    {
                        Logger.GetLogger.Debug("必修课程学习已结束！");
                        this.Stop();
                        return;
                    }
                    if(cancel.IsCancellationRequested)
                    {
                        Logger.GetLogger.Debug("必修课程学习已结束！");
                        this.Complete();
                        return;
                    }

                    if(courseList.List.All(s=>s.Detail?.Complete == true))
                    {

                        //所有课程都已完成学习, 即使学分没拿满, 也无法再进行学习
                        Logger.GetLogger.Warn("All courses completed, no new learning possible. ");
                        Complete();
                        return;
                    }

                    Logger.GetLogger.Info("Randomly choose a course to study.");
                    Random random = new Random();
                    int id = random.Next(0, courseList.Count);
                    if(courseList.List[id].Detail?.Complete == true)
                    {
                        await System.Threading.Tasks.Task.Delay(100);
                        continue;
                    }

                    var course = courseList.List[id];
                    Logger.GetLogger.Info($"Preparing for course: {course.ID}");
                    Logger.GetLogger.Debug($"Course Info: {course}");

                    await DoStudy(course);
                }
                
            }
            catch (Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Stop();
            }
        }


        private async System.Threading.Tasks.Task DoStudy(ElectiveCourse course)
        {
            try
            {
                await studyService.Start(course);
            }
            catch (Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Stop();
            }
        }

        private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            await new Service.CourseService(Fucker).GetIntegralInfo(courseList.List[0]);

            if (courseList.List[0].SumIntegral >= this.LimitIntegral)
                this.Complete();
        }


        protected override async Task<bool> Start()
        {
            var courseService = new Service.CourseService(Fucker);
            studyService = new Service.StudyService(Fucker, courseService, cancel.Token);

            timer = new System.Timers.Timer();
            timer.Interval = 10000;
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;

            await Fucker.GetCourseAppendix(courseList.List[0]);
            await courseService.GetIntegralInfo(courseList.List[0]);

            this.LimitIntegral = courseList.List[0].MaxIntegral;
            if (courseList.List[0].SumIntegral >= this.LimitIntegral)
                this.Complete();

            timer.Start();
            return true;
        }


        protected override bool Stop()
        {
            base.Stop();
            cancel.Cancel();
            timer?.Stop();
            return true;

        }

        protected override bool Complete()
        {
            base.Complete();
            cancel.Cancel();
            timer?.Stop();
            return true;
        }

    }
}
