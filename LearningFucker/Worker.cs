using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using LearningFucker.Models;

namespace LearningFucker
{
    public class Worker
    {
        public Worker()
        {
            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Interval = Fucker.POLLING_TIME;

            Fucker = new Fucker();
        }

        private Timer timer;
        private bool stopStudy = false;
        private bool studyProcessing = false;
        public AppInfo AppInfo { get; private set; }
        public Fucker Fucker { get; private set; }

        public TaskList TaskList { get; private set; }
        public UserStatistics UserStatistics { get; private set; }

        public User User { get; private set; }

        public Timer Timer { get => timer; }

        public List<Study> Studies { get; set; }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UserStatistics = await Fucker.GetMyTaskInfo();
            if (UserStatistics == null)
            {
                throw new Exception("获取用户任务完成信息时失败, 请重新打开程序重试!");
            }

            TaskList = await Fucker.GetTaskList("0");
            if (TaskList == null)
            {
                throw new Exception("获取任务信息时失败, 请重新打开程序重试!");
            }

            User = await Fucker.GetUserInfo();
            if (User == null)
                throw new Exception("用户已失效, 请重新登录;");

            if (!IsNeedStudy() && studyProcessing)
                this.StopStudy();
        }

        public async Task<bool> Login(string username, string password)
        {
            User = await Fucker.Login(username, password);
            if (User == null) return false;
            else
                return true;
        }

        public async System.Threading.Tasks.Task Init()
        {
            User = await Fucker.GetUserInfo();
            if (User == null)
                throw new Exception("用户已失效, 请重新登录;");

            AppInfo = await Fucker.GetAppInfo();
            if (AppInfo == null)
            {
                throw new Exception("获取App信息时失败, 请重新打开程序重试!");
            }

            TaskList = await Fucker.GetTaskList("0");
            if(TaskList == null)
            {
                throw new Exception("获取任务信息时失败, 请重新打开程序重试!");
            }

            UserStatistics = await Fucker.GetMyTaskInfo();
            if(UserStatistics == null)
            {
                throw new Exception("获取用户任务完成信息时失败, 请重新打开程序重试!");
            }

            

        }

        public void StartWork(List<int> tasks, bool parallel)
        {
            if (tasks == null || tasks.Count == 0)
                return;

            Timer.Start();

            foreach (var item in tasks)
            {
                switch(item)
                {
                    case 14:
                        if(IsNeedStudy() && studyProcessing == false)
                            StartStudy();
                        break;
                    case 2:
                        break;
                }
            }
        }

        public void StopWork()
        {

        }

        public async void StartStudy()
        {
            stopStudy = false;

            var courseList = await Fucker.GetCourseList(0, 100);
            if (courseList == null || courseList.List == null || courseList.List.Count == 0)
                throw new Exception("获取必修课程清单时出错, 请重新开启程序!");

            DoStudy(courseList);

        }

        public async void DoStudy(CourseList list)
        {
            studyProcessing = true;

            Random random = new Random();
            int id = random.Next(0, list.List.Count - 1);

            if (list.List[id].Detail != null && list.List[id].Detail.Complete)      //可能会死循环
            {
                DoStudy(list);
                return;
            }

            var course = list.List[id];

            await Fucker.GetCourseDetail(course);
            if (course.Detail == null)
                throw new Exception("获取课程详细信息时出错, 请重新开启程序!");
            await Fucker.GetCourseAppendix(course);
            if (course.Appendix == null)
                throw new Exception("获取课程附加信息时出错, 请重新开启程序!");

            foreach (var item in course.Detail.WareList)
            {
                if (stopStudy) return;

                var study = await Fucker.StartStudy(course, item);
                if (study == null)
                    throw new Exception("开始学习失败, 请重试!");
                if (Studies == null)
                    Studies = new List<Study>();

                Studies.Add(study);
                study.Start(Fucker);
                while (!study.Complete)
                {
                    await Fucker.GetStudyInfo(study);
                    if(study.Appendix.MaxStudyIntegral == study.StudyIntegral)  //学习任务结束
                    {
                        study.Stop();
                        StopStudy();

                    }
                    System.Threading.Thread.Sleep(10000);
                    if (stopStudy) return;
                }
            }

            DoStudy(list);
        }

        public async void StartExam()
        {

        }

        public async void DoExam(CourseList list)
        {
            Random random = new Random();
            int id = random.Next(0, list.List.Count - 1);

            var course = list.List[id];

            var examList = await Fucker.GetExamList(course);

        }

        public void StopStudy()
        {
            stopStudy = true;
            studyProcessing = false;
        }

        private bool IsNeedStudy()
        {
            if (TaskList == null || TaskList.List == null || TaskList.List.Count == 0) return false;

            var studyTask = TaskList.List.FirstOrDefault(s => s.TaskType == 14);
            if (studyTask == null) return false;

            if (!studyTask.Enabled || studyTask.IsHidden) return false;

            if (studyTask.LimitIntegral > studyTask.Integral)
                return true;
            else
                return false;
        }
        
    }
}
