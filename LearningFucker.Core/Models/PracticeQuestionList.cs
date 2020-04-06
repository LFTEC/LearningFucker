using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace LearningFucker.Models
{
    public class PracticeQuestionList
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("list")]
        public List<Question> Questions { get; set; }

        public PracticeResult Result { get; set; }
    }

    public class PracticeResult
    {
        /// <summary>
        /// 结果ID
        /// </summary>
        public string ResultId { get; set; }

        /// <summary>
        /// 答题状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 年
        /// </summary>
        public string Year { get; set; }

        /// <summary>
        /// 月
        /// </summary>
        public string Month { get; set; }


        /// <summary>
        /// 周
        /// </summary>
        public string Week { get; set; }
        public int TmSource { get; set; }

        /// <summary>
        /// 是否全部正确 0 错误, 1 正确
        /// </summary>
        public int IsAllRight { get; set; }

        /// <summary>
        /// 开始答题时间
        /// </summary>
        public string TimeBegin { get; set; }

        /// <summary>
        /// 结束答题时间
        /// </summary>
        public string TimeEnd { get; set; }

        /// <summary>
        /// 题数
        /// </summary>
        public int TotalNum { get; set; }

        /// <summary>
        /// 正确数
        /// </summary>
        public int CountRight { get; set; }

        /// <summary>
        /// 获得积分
        /// </summary>
        public decimal Integral { get; set; }

        /// <summary>
        /// 答题结果
        /// </summary>
        public string Gather { get; set; }

        
    }
}
