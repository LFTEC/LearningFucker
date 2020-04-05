using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningFucker.Models
{
    public class UserStatistics
    {
        /// <summary>
        /// 总学习任务数
        /// </summary>
        public int SumAllTask { get; set; }
        /// <summary>
        /// 今天未完成任务数
        /// </summary>
        public int TaskNoPass { get; set; }
        /// <summary>
        /// 今天已得积分
        /// </summary>
        public decimal TodayIntegral { get; set; }
        /// <summary>
        /// 总获得积分
        /// </summary>
        public decimal SumIntegral { get; set; }
        /// <summary>
        /// 积分当前排名
        /// </summary>
        public string IntegralRanking { get; set; }

        public decimal WeekIntegral { get; set; }
    }
}
