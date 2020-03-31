using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningFucker.Models;

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
            Random random = new Random();
            int id = random.Next(0, courseList.List.Count - 1);

            if (courseList.List[id].Detail != null && courseList.List[id].Detail.Complete)      //可能会死循环
            {
                DoWork();
                return;
            }

            var course = courseList.List[id];

            await Fucker.GetCourseDetail(course);
            if (course.Detail == null)
                throw new Exception("获取课程详细信息时出错, 请重新开启程序!");
            await Fucker.GetCourseAppendix(course);
            if (course.Appendix == null)
                throw new Exception("获取课程附加信息时出错, 请重新开启程序!");

            if (course.Detail.WareList != null && course.Detail.WareList.Count > 0)
                DoStudy(course, course.Detail.WareList[0]);
            else
                DoWork();
        }

        private async void DoStudy(Course course, WareDetail item)
        {
            var study = await Fucker.StartStudy(course, item);
            if (study == null)
                throw new Exception("开始学习失败, 请重试!");
            if (Studies == null)
                studies = new List<Study>();

            Studies.Add(study);

            study.Start(Fucker);
            study.StudyComplete = new Action<Study>(s=> {
                if(this.TaskStatus == TaskStatus.Working)
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

        private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
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

        public override bool Start(Fucker fucker)
        {
            if (!base.Start(fucker)) return false;

            Start();
            return true;
        }


        private async void Start()
        {
            var courseList = await Fucker.GetCourseList(0, 100);
            if (courseList == null || courseList.List == null || courseList.List.Count == 0)
                throw new Exception("not implemented");

            this.courseList = courseList;
            await Fucker.GetCourseAppendix(courseList.List[0]);
            if (courseList.List[0].Appendix == null)
                throw new Exception("获取课程附加信息时出错, 请重新开启程序!");

            this.TaskForWork.LimitIntegral = courseList.List[0].Appendix.MaxStudyIntegral;
            timer.Start();
            DoWork();
        }

        public override bool Stop()
        {
            TaskStatus = TaskStatus.Stopping;
            if (Studies != null)
            {
                foreach( var item in Studies.Where(s=>s.Complete == false))
                {
                    item.Stop();
                }
            }

            if (timer.Enabled)
                timer.Stop();

            TaskStatus = TaskStatus.Stopped;
            TaskForWork.TaskStatus = TaskStatus.Stopped;
            return true;
        }

        protected override bool Complete()
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

            TaskStatus = TaskStatus.Completed;
            TaskForWork.TaskStatus = TaskStatus.Completed;
            return true;
        }
    }
}
