using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LearningFucker.Models
{
    public class TaskList
    {
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
        [JsonProperty(PropertyName = "list")]
        public List<Task> List { get; set; }
    }

    public class Task
    {
        /// <summary>
        /// 任务id
        /// </summary>
        public int TaskType { get; set; }
        /// <summary>
        /// 任务描述
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 每日可得积分
        /// </summary>
        public decimal LimitIntegral { get; set; }
        /// <summary>
        /// 每日已得积分
        /// </summary>
        public decimal Integral { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Explain { get; set; }
        /// <summary>
        /// 每天
        /// </summary>
        public string Period { get; set; }
        /// <summary>
        /// 有效
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool IsHidden { get; set; }
        public int TaskCategory { get; set; }

        /// <summary>
        /// 未完成任务数
        /// </summary>
        public int UncompeletedItemCount { get; set; }

        public bool IsSelect { get; set; }

        public LearningFucker.Handler.TaskStatus TaskStatus { get; set; }
        

        public void Clone(Task task)
        {
            this.TaskType = task.TaskType;
            this.Name = task.Name;
            this.LimitIntegral = task.LimitIntegral;
            this.Integral = task.Integral;
            this.Explain = task.Explain;
            this.Period = task.Period;
            this.Enabled = task.Enabled;
            this.IsHidden = task.IsHidden;
            this.TaskCategory = task.TaskCategory;
            this.UncompeletedItemCount = task.UncompeletedItemCount;
        }

    }
}
