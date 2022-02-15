using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LearningFucker.Handler
{
    public abstract class TaskHandlerBase : ITaskHandler
    {
        public async Task<bool> Start(Fucker fucker)
        {
            try
            {

                this.Fucker = fucker;
                if (TaskStatus != TaskStatus.Initial)
                    return false;
                TaskStatus = TaskStatus.Working;

                if (!await Start())
                    return false;
                else
                {
                    await DoWork();
                    return true;
                }
            }
            catch(Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                return false;
            }
                
        }

        protected abstract Task<bool> Start();

        protected virtual bool Stop()
        {
            if (TaskStatus != TaskStatus.Working)
                return false;
            TaskStatus = TaskStatus.Stopping;

            return true;
        }

        public abstract Task DoWork();

        protected virtual bool Complete()
        {
            if (TaskStatus != TaskStatus.Working)
                return false;
            TaskStatus = TaskStatus.Completed;
            return true;
        }

        public TaskHandlerBase(CancellationToken token, Models.Task task)
        {
            Task = task;
            CancellationToken = token;
            TaskStatus = TaskStatus.Initial;
        }

        private TaskStatus taskStatus;
        private Fucker fucker;

        public Action<object, TaskStatus> StatusChanged { get; set; }

        protected CancellationToken CancellationToken { get; set; }


        protected Fucker Fucker { get => fucker; set => fucker = value; }

        public TaskStatus TaskStatus { get => taskStatus; protected set { taskStatus = value; StatusChanged?.Invoke(this, value);  } }

        public decimal LimitIntegral { get; set; }
        public decimal Integral { get; set; }

        public LearningFucker.Models.Task Task { get; set; }
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
