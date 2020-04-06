using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LearningFucker.Models
{
    public class CombatList
    {
        [JsonProperty("RankList")]
        public List<Gladiator> Gladiators { get; set; }

        [JsonProperty("pkUserInfo")]
        public Gladiator Myself { get; set; }
    }

    public class Gladiator
    {
        /// <summary>
        /// 战力积分
        /// </summary>
        public int Combat { get; set; }

        /// <summary>
        /// 对战次数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 获胜次数
        /// </summary>
        public int WinCount { get; set; }

        /// <summary>
        /// 都是0
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 部门信息
        /// </summary>
        public string CompanyId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string CompanyName { get; set; }
    }

    public class Arena
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string TmFrom { get; set; }

        /// <summary>
        /// 题目数量
        /// </summary>
        public int TmNumber { get; set; }

        /// <summary>
        /// 答题时间每题, 秒
        /// </summary>
        public int PKTime { get; set; }

        /// <summary>
        /// 竞技场编号
        /// </summary>
        public string GroupId { get; set; }

        public List<Gladiator> BothSides { get; set; }

        /// <summary>
        /// 战斗回合
        /// </summary>
        public List<Round> Rounds { get; set; }

        /// <summary>
        /// PK结果
        /// </summary>
        public List<CombatResult> Results { get; set; }
    }

    public class Round
    {
        /// <summary>
        /// 每回合战绩, 0是失败, 1是成功, 2是未开始
        /// </summary>
        public List<List<int>> AnswerResult { get; set; }

        public string Status { get; set; }

        /// <summary>
        /// 第X回合
        /// </summary>
        public int CurrentIndex { get; set; }

        /// <summary>
        /// 试题
        /// </summary>
        public Question Question { get; set; }
    }

    public class CombatResult
    {
        /// <summary>
        /// 答题时间(秒)
        /// </summary>
        public int UseDate { get; set; }

        /// <summary>
        /// 正确数量
        /// </summary>
        public int RightCount { get; set; }

        /// <summary>
        /// pk获得积分
        /// </summary>
        public decimal PKScroe { get; set; }

        /// <summary>
        /// 是否取胜(0取胜, 1失败)
        /// </summary>
        public int IsWin { get; set; }

        /// <summary>
        /// 当前战力
        /// </summary>
        public int Combat { get; set; }

        /// <summary>
        /// 本次获得战力
        /// </summary>
        public int CombatChange { get; set; }

        [JsonProperty("pkUserInfo")]
        public ResultGladiator Gladiator { get; set; }

    }

    public class ResultGladiator
    {
        public string UserName { get; set; }
        public string RealName { get; set; }
    }


}
