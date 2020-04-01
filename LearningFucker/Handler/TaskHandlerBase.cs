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
            TaskStatus = TaskStatus.Working;
            TaskForWork.StartTime = DateTime.Now;
            TaskForWork.TaskStatus = TaskStatus.Working;

            return true;
        }

        public abstract bool Stop();

        public abstract void DoWork();

        protected abstract bool Complete();

        public TaskHandlerBase()
        {
        }

        private TaskStatus taskStatus;
        private TaskForWork taskForWork;
        private Fucker fucker;
        public TaskForWork TaskForWork
        {
            get => taskForWork;
            set { taskForWork = value; }
        }

        protected Fucker Fucker { get => fucker; set => fucker = value; }

        public TaskStatus TaskStatus { get => taskStatus; protected set => taskStatus = value; }
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

        public Action<TaskForWork> Completed;
        private TaskStatus taskStatus;
        public TaskStatus TaskStatus { get=>taskStatus;
            set {
                taskStatus = value;
                if (taskStatus == TaskStatus.Completed)
                {
                    Completed(this);
                }
            }
        }

        public DateTime StartTime { get; internal set; }
        public DateTime EndTime { get; internal set; }


        public void SetHandler(ITaskHandler handler)
        {
            Handler = handler;
            ((TaskHandlerBase)Handler).TaskForWork = this;
        }

        public bool Start(Fucker fucker)
        {
            if (Handler != null && Handler.TaskStatus != TaskStatus.Completed && Handler.TaskStatus != TaskStatus.Working)
            {
                return Handler.Start(fucker);
            }
            else
                return false;
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
