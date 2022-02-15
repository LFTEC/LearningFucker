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

            WorkList = new List<Handler.TaskHandlerBase>();

            User = new User();
            TaskList = new List<Models.Task>();

            cancellation = new System.Threading.CancellationTokenSource();
        }

        private Timer timer;
        private bool studyProcessing = false;
        public AppInfo AppInfo { get; private set; }

        private System.Threading.CancellationTokenSource cancellation;
        public Fucker Fucker { get; private set; }

        public event EventHandler WorkStarted;
        public event EventHandler WorkStopped;

        public List<LearningFucker.Models.Task> TaskList { get; set; }
        public UserStatistics UserStatistics { get; private set; }

        public CourseList CourseList { get; private set; }
        public ElectiveCourseList ElectiveCourseList { get; private set; }

        public User User { get; private set; }

        public Timer Timer { get => timer; }

        public List<Study> Studies { get; set; }

        private List<TaskHandlerBase> WorkList { get; set; }

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

        public async Task<List<int>> CanLearnedAsync(List<int> studyPlan)
        {
            await Init();

            return studyPlan.Where(plan => TaskList.Where(task => task.Integral < task.LimitIntegral && task.TaskType == plan).Any()).ToList();
        }

        public async Task<List<int>> CanLearnedAsync()
        {
            List<int> studyPlan = LearningFucker.Models.TaskList.AvailableTasks.ToList();
            return await CanLearnedAsync(studyPlan);
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

        public async System.Threading.Tasks.Task StartWork(List<int> tasks, bool parallel)
        {
            if (tasks == null || tasks.Count == 0)
                return;

            this.Parallel = parallel;

            this.Say("开始进行学习任务...");

            Timer.Start();

            var courseService = new Service.CourseService(Fucker);
            this.CourseList = await courseService.GetCourseList();
            this.ElectiveCourseList = await courseService.GetElectiveCourseListAsync();

            LaunchWorkList(tasks);
            WorkStarted?.Invoke(this, new EventArgs());

            if (parallel)
            {
                System.Threading.Tasks.Task[] t = new System.Threading.Tasks.Task[WorkList.Count];
                foreach (var (work,i) in WorkList.Select((work,i)=>(work,i)))
                {
                    t[i] = new System.Threading.Tasks.Task(async () =>
                    {
                        await work.Start(Fucker);
                    });

                    t[i].Start();
                }

                System.Threading.Tasks.Task.WaitAll(t);
            }
            else
            {
                foreach (var work in WorkList)
                {
                    await work.Start(Fucker);
                }
            }

            
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
                        TaskHandlerBase handler = new StudyHandler(CourseList, cancellation.Token, task);
                        handler.StatusChanged += new Action<object, Handler.TaskStatus>(WorkItemStatusChanged);
                        WorkList.Add(handler);

                        handler = new ExamHandler(CourseList, cancellation.Token, task);
                        handler.StatusChanged += new Action<object, Handler.TaskStatus>(WorkItemStatusChanged);
                        WorkList.Add(handler);

                        break;
                    case 2:
                        task = TaskList.FirstOrDefault(s => s.TaskType == item);
                        handler = new ElectiveHandler(ElectiveCourseList, cancellation.Token, task);
                        handler.StatusChanged += new Action<object, Handler.TaskStatus>(WorkItemStatusChanged);
                        WorkList.Add(handler);

                        handler = new ExerciseHandler(ElectiveCourseList, cancellation.Token, task);
                        handler.StatusChanged += new Action<object, Handler.TaskStatus>(WorkItemStatusChanged);
                        WorkList.Add(handler);
                        break;
                    case 13:
                        task = TaskList.FirstOrDefault(s => s.TaskType == item);
                        handler = new PKHandler(cancellation.Token, task);
                        handler.StatusChanged += new Action<object, Handler.TaskStatus>(WorkItemStatusChanged);
                        WorkList.Add(handler);
                        break;

                    case 11:
                        task = TaskList.FirstOrDefault(s => s.TaskType == item);
                        handler = new BreakthroughHandler(cancellation.Token, task);
                        handler.StatusChanged += new Action<object, Handler.TaskStatus>(WorkItemStatusChanged);
                        WorkList.Add(handler);
                        break;
                    case 7:
                        task = TaskList.FirstOrDefault(s => s.TaskType == item);
                        handler = new WeeklyPracticeHandler(cancellation.Token, task);
                        handler.StatusChanged += new Action<object, Handler.TaskStatus>(WorkItemStatusChanged);
                        WorkList.Add(handler);
                        
                        break;
                }
            }
        }


        private void WorkItemStatusChanged(object sender, Handler.TaskStatus status)
        {
            if (WorkList.All(work => work.TaskStatus == Handler.TaskStatus.Completed || work.TaskStatus == Handler.TaskStatus.Stopped))
            {
                if (!WorkList.Any(work => work.TaskStatus == Handler.TaskStatus.Stopped))
                    this.Say("所有任务已完成");

                WorkStopped?.Invoke(this, new EventArgs());
            }
                

        }
        

        public void StopWork()
        {
            this.Say("所有进行中任务停止中...");

            if (timer.Enabled)
                timer.Stop();

            
            cancellation.Cancel();

            
        }



        
    }
}
