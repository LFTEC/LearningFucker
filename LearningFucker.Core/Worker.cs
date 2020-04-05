using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using LearningFucker.Models;
using LearningFucker.Handler;

namespace LearningFucker
{
    public class Worker
    {
        public Worker()
        {
            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = true;
            timer.Interval = 10000;

            Fucker = new Fucker(this);

            WorkList = new List<TaskForWork>();

            User = new User();
            TaskList = new List<Models.Task>();
        }

        private Timer timer;
        private bool studyProcessing = false;
        public AppInfo AppInfo { get; private set; }
        public Fucker Fucker { get; private set; }

        public event EventHandler WorkStarted;
        public event EventHandler WorkStopped;

        public List<LearningFucker.Models.Task> TaskList { get; set; }
        public UserStatistics UserStatistics { get; private set; }

        public User User { get; private set; }

        public Timer Timer { get => timer; }

        public List<Study> Studies { get; set; }

        private List<TaskForWork> WorkList { get; set; }

        public Action<Worker> TaskRefresed;

        public Action<object, string> OnSaying { get; set; }
        public Action<object, string> OnReportingError { get; set; }
        public bool Parallel { get; private set; }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Refresh();
        }

        public async Task<bool> Login(string username, string password)
        {
            var user = await Fucker.Login(username, password);
            if (user == null) return false;
            else
            {
                User.UserId = username;
                User.Password = password;
                User.Clone(user);
                return true;
            }
        }

        public void Say(string text)
        {
            OnSaying?.Invoke(this, text);
        }

        public void ReportError(string errText)
        {
            OnReportingError?.Invoke(this, errText);
        }

        public async System.Threading.Tasks.Task Init()
        {

            AppInfo = await Fucker.GetAppInfo();
            if (AppInfo == null)
            {
                throw new Exception("获取App信息时失败, 请重新打开程序重试!");
            }

            await RefreshTaskList();

            UserStatistics = await Fucker.GetMyTaskInfo();
            if(UserStatistics == null)
            {
                throw new Exception("获取用户任务完成信息时失败, 请重新打开程序重试!");
            }

            if (TaskRefresed != null)
                TaskRefresed(this);

        }

        public async void Refresh()
        {
            await RefreshTaskList();

            UserStatistics = await Fucker.GetMyTaskInfo();
            if (UserStatistics == null)
            {
                throw new Exception("获取用户任务完成信息时失败, 请重新打开程序重试!");
            }

            if (TaskRefresed != null)
                TaskRefresed(this);

        }

        private async System.Threading.Tasks.Task RefreshTaskList()
        {
            var taskList = await Fucker.GetTaskList("0");
            if (taskList == null)
            {
                throw new Exception("获取任务信息时失败, 请重新打开程序重试!");
            }
            foreach (var item in taskList.List)
            {
                var task = this.TaskList.FirstOrDefault(s => s.TaskType == item.TaskType);
                if (task == null)
                {
                    TaskList.Add(item);
                }
                else
                    task.Clone(item);
            }

            foreach (var item in TaskList)
            {
                item.TaskStatus = Handler.TaskStatus.Stopped;
                var worklist = WorkList.Where(s => s.Task == item);
                if(worklist.Count() > 0 && worklist.All(s => s.TaskStatus == Handler.TaskStatus.Completed) )
                {
                    item.TaskStatus = Handler.TaskStatus.Completed;
                }

                if(worklist.Any(s=>s.TaskStatus == Handler.TaskStatus.Working))
                {
                    item.TaskStatus = Handler.TaskStatus.Working;
                }


            }
        }

        public void StartWork(List<int> tasks, bool parallel)
        {
            if (tasks == null || tasks.Count == 0)
                return;

            this.Parallel = parallel;

            this.Say("开始进行学习任务...");

            Timer.Start();

            LaunchWorkList(tasks);

            WorkList[0].Start(Fucker);

            WorkStarted?.Invoke(this, new EventArgs());
        }

        private void LaunchWorkList(List<int> tasks)
        {
            WorkList.Clear();
            LearningFucker.Models.Task task;
            foreach (var item in tasks)
            {
                switch (item)
                {
                    case 14:
                        task = TaskList.FirstOrDefault(s => s.TaskType == item);
                        TaskForWork taskForWork = new TaskForWork(task, new StudyHandler());
                        taskForWork.OnCompleted += new Action<TaskForWork>(WorkItemCompleted);
                        taskForWork.OnStopped += TaskForWork_OnStopped;
                        WorkList.Add(taskForWork);

                        taskForWork = new TaskForWork(task, new ExamHandler());
                        taskForWork.OnCompleted += new Action<TaskForWork>(WorkItemCompleted);
                        taskForWork.OnStopped += TaskForWork_OnStopped;
                        WorkList.Add(taskForWork);

                        break;
                    case 2:
                        task = TaskList.FirstOrDefault(s => s.TaskType == item);
                        taskForWork = new TaskForWork(task, new ElectiveHandler());
                        taskForWork.OnCompleted += new Action<TaskForWork>(WorkItemCompleted);
                        taskForWork.OnStopped += TaskForWork_OnStopped;
                        WorkList.Add(taskForWork);

                        taskForWork = new TaskForWork(task, new ExerciseHandler());
                        taskForWork.OnCompleted += new Action<TaskForWork>(WorkItemCompleted);
                        taskForWork.OnStopped += TaskForWork_OnStopped;
                        WorkList.Add(taskForWork);
                        break;
                    case 13:
                        task = TaskList.FirstOrDefault(s => s.TaskType == item);
                        taskForWork = new TaskForWork(task.LimitIntegral, task.Integral, task, new PKHandler());
                        taskForWork.OnCompleted += new Action<TaskForWork>(WorkItemCompleted);
                        taskForWork.OnStopped += TaskForWork_OnStopped;
                        WorkList.Add(taskForWork);
                        break;

                    case 11:
                        task = TaskList.FirstOrDefault(s => s.TaskType == item);
                        taskForWork = new TaskForWork(task.LimitIntegral, task.Integral, task, new BreakthroughHandler());
                        taskForWork.OnCompleted += new Action<TaskForWork>(WorkItemCompleted);
                        taskForWork.OnStopped += TaskForWork_OnStopped;
                        WorkList.Add(taskForWork);
                        break;
                }
            }
        }

        private void TaskForWork_OnStopped(TaskForWork obj)
        {
            this.Say("所有任务已停止.");
            this.WorkStopped?.Invoke(this, new EventArgs());

        }

        private void WorkItemCompleted(TaskForWork workItem)
        {
            var index = WorkList.IndexOf(workItem);
            if (index == WorkList.Count - 1)
            {
                this.Say("学习任务已全部结束");
                this.WorkStopped?.Invoke(this, new EventArgs());
            }
            else
            {
                index++;
                WorkList[index].Start(Fucker);
            }
        }

        public void StopWork()
        {
            var list = WorkList.Where(s => s.TaskStatus == Handler.TaskStatus.Working);
            if (list.Count() == 0) return;

            if (timer.Enabled)
                timer.Stop();

            this.Say("所有进行中任务停止中...");
            foreach (var item in list)
            {
                item.Stop();
            }
            
        }



        
    }
}
