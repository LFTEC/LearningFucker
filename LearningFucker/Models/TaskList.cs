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

    }
}
