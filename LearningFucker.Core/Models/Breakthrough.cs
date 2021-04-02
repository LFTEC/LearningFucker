using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LearningFucker.Models
{
    public class BreakthroughList
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("list")]
        public List<Breakthrough> List { get; set; }
    }

    public class Breakthrough
    {
        /// <summary>
        /// 闯关项目编号
        /// </summary>
        public string PointLevelId { get; set; }


        /// <summary>
        /// 闯关项目名称
        /// </summary>
        public string PointName { get; set; }

        ///// <summary>
        ///// 级别
        ///// </summary>
        //public int Level { get; set; }

        /// <summary>
        /// 问题数量
        /// </summary>
        public int QuestionNumber { get; set; }

        ///// <summary>
        ///// 需答对数量
        ///// </summary>
        //public int PassQuestionNumber { get; set; }

        /// <summary>
        /// 答题时间
        /// </summary>
        public int AnswerTime { get; set; }

        public bool LimitTime { get; set; }
        public int MaxPointNumber { get; set; }
        public int UserPointNumber { get; set; }


        ///// <summary>
        ///// 备注
        ///// </summary>
        //public string PointRemark { get; set; }

        /// <summary>
        /// 是否已通过
        /// </summary>
        public bool CanJoin { get; set; }

        ///// <summary>
        ///// 项目积分
        ///// </summary>
        //public decimal Integral { get; set; }

        /// <summary>
        /// 试题清单
        /// </summary>
        [JsonProperty("list")]
        public List<Question> Questions { get; set; }

        public BreakthroughResult Result { get; set; }
    }
}
