using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningFucker.Models;

namespace LearningFucker.Handler
{
    public class ElectiveHandler : TaskHandlerBase
    {
        public ElectiveHandler()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 10000;
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
        }

        private Models.ElectiveCourseList courseList;
        private PropertyList propertyList;
        private List<Models.Study> studies;
        private System.Timers.Timer timer;

        public List<Study> Studies { get => studies; }

        public async override void DoWork()
        {
            Random random = new Random();
            int id = random.Next(0, propertyList.List[0].SubNodes.Count - 1);

            if (propertyList.List[0].SubNodes[id] == null)      //可能会死循环
            {
                DoWork();
                return;
            }

            var context = propertyList.List[0].SubNodes[id];

            courseList = await Fucker.GetElectiveCourseList(context);


            DoContext();
        }

        private async void DoContext()
        {
            Random random = new Random();
            int id = random.Next(0, courseList.List.Count - 1);

            if (courseList.List[id].Detail != null && courseList.List[id].Detail.Complete)      //可能会死循环
            {
                DoContext();
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
                DoContext();
        }

        private async void DoStudy(ElectiveCourse course, WareDetail item)
        {
            var study = await Fucker.StartStudy(course, item);
            if (study == null)
                throw new Exception("开始学习失败, 请重试!");
            if (Studies == null)
                studies = new List<Study>();

            if (await Fucker.GetStudyInfo(study))
                study.InitIntegral = study.StudyIntegral;

            Studies.Add(study);

            study.Start(Fucker);
            study.StudyComplete = new Action<Study>(s => {
                if (this.TaskStatus == TaskStatus.Working)
                {
                    var index = course.Detail.WareList.IndexOf(item);
                    if (index == course.Detail.WareList.Count - 1)
                        DoContext();
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
            var propertyList = await Fucker.GetPropertyList();
            if (propertyList == null || propertyList.List == null || propertyList.List.Count == 0)
                throw new Exception("not implemented");

            this.propertyList = propertyList;

            var courselist = await Fucker.GetElectiveCourseList(propertyList.List[0].SubNodes[0]);
            if(courselist == null || courselist.List == null || courselist.List.Count == 0)
                throw new Exception("not implemented");

            
            await Fucker.GetCourseAppendix(courselist.List[0]);
            if (courselist.List[0].Appendix == null)
                throw new Exception("获取课程附加信息时出错, 请重新开启程序!");

            this.TaskForWork.LimitIntegral = courselist.List[0].Appendix.MaxStudyIntegral;
            timer.Start();
            DoWork();
        }

        public override bool Stop()
        {
            TaskStatus = TaskStatus.Stopping;
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
