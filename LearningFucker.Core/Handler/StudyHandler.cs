using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningFucker.Models;
using System.Diagnostics;
using NLog.Fluent;

namespace LearningFucker.Handler
{
    public class StudyHandler : TaskHandlerBase
    {
        public StudyHandler()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 10000;
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
        }        

        private Models.CourseList courseList;
        private List<Models.Study> studies;
        private System.Timers.Timer timer;

        public List<Study> Studies { get => studies; }

        public async override void DoWork()
        {
            try
            {
                if(courseList.List.All(s=>s.Detail != null && s.Detail.Complete))
                {
                    //所有课程都已完成学习, 即使学分没拿满, 也无法再进行学习
                    Logger.GetLogger.Warn("All courses completed, no new learning possible. ");
                    Complete();
                    return;
                }

                Logger.GetLogger.Info("Randomly choose a course to study.");
                Random random = new Random();
                int id = random.Next(0, courseList.List.Count - 1);

                if (courseList.List[id].Detail != null && courseList.List[id].Detail.Complete)     
                {
                    System.Threading.Thread.Sleep(100);
                    DoWork();
                    return;
                }


                var course = courseList.List[id];
                Logger.GetLogger.Info($"Preparing for course: {course.ProjID}");
#if DEBUG
                Logger.GetLogger.Debug($"Course Info: {course}");
#endif

                await Fucker.GetCourseDetail(course);
                await Fucker.GetCourseAppendix(course);

                if (course.Detail.WareList != null && course.Detail.WareList.Count > 0)
                    DoStudy(course, course.Detail.WareList[0]);
                else
                    DoWork();
            }
            catch(Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Stop();
            }
        }

        private async void DoStudy(Course course, WareDetail item)
        {
            try
            {
                if(item.AllowIntegral <= 0)
                {
                    if (this.TaskStatus == TaskStatus.Working)
                    {
                        var index = course.Detail.WareList.IndexOf(item);
                        if (index == course.Detail.WareList.Count - 1)
                            DoWork();
                        else
                        {
                            index++;
                            DoStudy(course, course.Detail.WareList[index]);
                        }
                    }
                    return;
                }

                var study = await Fucker.StartStudy(course, item);
                if (Studies == null)
                    studies = new List<Study>();

                if (await Fucker.GetStudyInfo(study))
                    study.InitIntegral = study.StudyIntegral;

                Studies.Add(study);

                study.Start(Fucker);
                study.StudyComplete = new Action<Study>(s =>
                {
                    if (this.TaskStatus == TaskStatus.Working)
                    {
                        var index = course.Detail.WareList.IndexOf(item);
                        if (index == course.Detail.WareList.Count - 1)
                            DoWork();
                        else
                        {
                            index++;
                            DoStudy(course, course.Detail.WareList[index]);
                        }
                    }
                });
            }
            catch(Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Stop();
            }
        }

        private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if(Studies == null)
                {
                    Stop();
                    return;
                }

                var study = Studies.FirstOrDefault(s => s.Complete == false);
                if (study == null)
                    Stop();
                else
                {
                    await Fucker.GetStudyInfo(study);
                    this.TaskForWork.Integral = study.StudyIntegral;

                    if (this.TaskForWork.LimitIntegral == this.TaskForWork.Integral)  //学习任务结束
                    {
                        Complete();
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


        private async void Start()
        {
            try
            {
                var courseList = await Fucker.GetCourseList(0, 100);

                this.courseList = courseList;
                await Fucker.GetCourseAppendix(courseList.List[0]);

                this.TaskForWork.LimitIntegral = courseList.List[0].Appendix.MaxStudyIntegral;
                timer.Start();

                Logger.GetLogger.Info("Preparation for compulsory courses. ");
                DoWork();
            }
            catch(Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Stop();
            }
        }

        public override bool Stop()
        {
            if (base.Stop())
            {
                if (Studies != null)
                {
                    foreach (var item in Studies.Where(s => s.Complete == false))
                    {
                        item.Stop();
                    }
                }

                if (timer.Enabled)
                    timer.Stop();

                TaskStatus = TaskStatus.Stopped;
                return true;
            }
            else
                return false;
            
        }

        protected override bool Complete()
        {
            if (base.Complete())
            {
                if (Studies != null)
                {
                    foreach (var item in Studies.Where(s => s.Complete == false))
                    {
                        item.Stop();
                    }
                }

                if (timer.Enabled)
                    timer.Stop();

                return true;
            }
            else
                return false;
            
        }
    }
}
