using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningFucker.Handler
{
    public abstract class TaskHandlerBase : ITaskHandler
    {
        public virtual bool Start(Fucker fucker)
        {
            this.Fucker = fucker;
            if (TaskStatus != TaskStatus.Initial)
                return false;
            TaskStatus = TaskStatus.Working;

            return true;
        }

        public virtual bool Stop()
        {
            if (TaskStatus != TaskStatus.Working)
                return false;
            TaskStatus = TaskStatus.Stopping;

            return true;
        }

        public abstract void DoWork();

        protected virtual bool Complete()
        {
            if (TaskStatus != TaskStatus.Working)
                return false;
            TaskStatus = TaskStatus.Completed;
            return true;
        }

        public TaskHandlerBase()
        {
        }

        private TaskStatus taskStatus;
        private TaskForWork taskForWork;
        private Fucker fucker;

        public Action<object, TaskStatus> StatusChanged { get; set; }
        public TaskForWork TaskForWork
        {
            get => taskForWork;
            set { taskForWork = value; }
        }

        protected Fucker Fucker { get => fucker; set => fucker = value; }

        public TaskStatus TaskStatus { get => taskStatus; protected set { taskStatus = value; StatusChanged?.Invoke(this, value);  } }
    }

    public class TaskForWork
    {
        public TaskForWork(decimal limitIntegral, decimal Integral, LearningFucker.Models.Task task )
        {
            this.LimitIntegral = limitIntegral;
            this.Integral = Integral;
            this.Task = task;
            TaskStatus = TaskStatus.Initial;
        }

        public TaskForWork(LearningFucker.Models.Task task) : this(0,0, task)
        {

        }

        public TaskForWork(decimal limitIntegral, decimal Integral, LearningFucker.Models.Task task, ITaskHandler handler)
            : this(limitIntegral, Integral, task)
        {
            SetHandler(handler);
        }

        public TaskForWork(LearningFucker.Models.Task task, ITaskHandler handler)
            : this(task)
        {
            SetHandler(handler);
        }

        public int WorkId { get; set; }
        public decimal LimitIntegral { get; set; }
        public decimal Integral { get; set; }
        public ITaskHandler Handler { get; internal set; }
        public LearningFucker.Models.Task Task { get; }

        public event Action<TaskForWork> OnCompleted;

        private TaskStatus taskStatus;
        public TaskStatus TaskStatus { get=>taskStatus;
            set {
                taskStatus = value;
                if (taskStatus == TaskStatus.Completed)
                    OnCompleted?.Invoke(this);
            }
            
        }

        public DateTime StartTime { get; internal set; }
        public DateTime EndTime { get; internal set; }


        public void SetHandler(ITaskHandler handler)
        {
            Handler = handler;
            ((TaskHandlerBase)Handler).TaskForWork = this;

            handler.StatusChanged += new Action<object, TaskStatus>(( s, status) =>
            {
                this.TaskStatus = status;
                if(TaskStatus == TaskStatus.Working)
                {
                    this.StartTime = DateTime.Now;
                }
                else if(TaskStatus == TaskStatus.Completed )
                {
                    this.EndTime = DateTime.Now;
                }
            });
        }

        public bool Start(Fucker fucker)
        {
            return Handler.Start(fucker);
        }

        public bool Stop()
        {
            return Handler.Stop();
        }
    }

    public enum TaskStatus
    {
        /// <summary>
        /// 未开始
        /// </summary>
        Initial = 0,
        /// <summary>
        /// 进行中
        /// </summary>
        Working = 1,
        /// <summary>
        /// 停止进程中...
        /// </summary>
        Stopping = 2,
        /// <summary>
        /// 停止
        /// </summary>
        Stopped = 3,
        /// <summary>
        /// 已完成
        /// </summary>
        Completed = 4
    }
}
