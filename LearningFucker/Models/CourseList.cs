using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LearningFucker.Models
{
    public class CourseList
    {
        [JsonProperty(PropertyName = "allowList")]
        public List<Course> List { get; set; }
    }

    public class ElectiveCourseList
    {
        [JsonProperty("recordCount")]
        public int Count { get; set; }

        [JsonProperty("courses")]
        public List<ElectiveCourse> List { get; set; }
    }

    public class Course
    {
        /// <summary>
        /// 课程id
        /// </summary>
        public string ProjID { get; set; }
        /// <summary>
        /// 课程描述
        /// </summary>
        public string ProjName { get; set; }

        /// <summary>
        /// 课程类型(暂时必修是3, 选修是0)
        /// </summary>
        public string ProjType { get; set; }
        public string Proj2ID { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public string ValidateTimeB { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public string ValidateTimeE { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string AddTime { get; set; }

        public CourseDetail Detail { get; set; }

        public CourseAppendix Appendix { get; set; }
    }

    public class ElectiveCourse
    {
        public string ID { get; set; }

        public string Proj2ID { get; set; }

        /// <summary>
        /// 课程类型(暂时必修是3, 选修是0)
        /// </summary>
        public string ProjType { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }

        /// <summary>
        /// 已审批为1
        /// </summary>
        public int Audited { get; set; }

        /// <summary>
        /// 已学习次数
        /// </summary>
        public int Review { get; set; }

        /// <summary>
        /// 需学习时间
        /// </summary>
        public int ClassHour { get; set; }

        [JsonProperty("Valid_bTime")]
        public string ValidateTimeB { get; set; }

        [JsonProperty("Valid_eTime")]
        public string ValidateTimeE { get; set; }

        public string Creator { get; set; }

        public string CreateTime { get; set; }
        public string Company { get; set; }
        public string Author { get; set; }

        /// <summary>
        /// 评论数量
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// 点赞数量
        /// </summary>
        public int PraiseCount { get; set; }

        public CourseDetail Detail { get; set; }

        public CourseAppendix Appendix { get; set; }

        /// <summary>
        /// 练习卷清单
        /// </summary>
        public List<Exercise> Exercises { get; set; }
    }

    public class CourseDetail
    {
        public bool IsForceOrder { get; set; }
        /// <summary>
        /// 子课程数量
        /// </summary>
        public int WareCount { get; set; }
        /// <summary>
        /// 需学习时长(秒)
        /// </summary>
        public int TotalMinute { get; set; }
        /// <summary>
        /// 已学习时长(秒)
        /// </summary>
        public int DoneMinute { get; set; }
        /// <summary>
        /// 进度条
        /// </summary>
        public decimal Progress { get; set; }
        /// <summary>
        /// 问题数量?
        /// </summary>
        public int QuestionCount { get; set; }
        /// <summary>
        /// 已学习用户数
        /// </summary>
        public int StudyUserCount { get; set; }

        public List<WareDetail> WareList { get; set; }

        /// <summary>
        /// 本次学习时长
        /// </summary>
        public int StudyDuration { get; set; }

        public bool Complete { get; set; }

    }

    public class WareDetail
    {
        /// <summary>
        /// 子课程id
        /// </summary>
        public string WareId { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 需学习时长(秒)
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// 已学习时长
        /// </summary>
        public int AlreadyStudyTime { get; set; }
        /// <summary>
        /// 顺序
        /// </summary>
        public int OrderIndex { get; set; }
        /// <summary>
        /// 进度条
        /// </summary>
        public decimal Progress { get; set; }
        /// <summary>
        /// 问题数量?
        /// </summary>
        public int QuestionCount { get; set; }

        /// <summary>
        /// 学习时长
        /// </summary>
        public int StudyDuration { get; set; }

        public bool Complete { get; set; }

    }

    public class CourseAppendix
    {
        /// <summary>
        /// module
        /// </summary>
        public string MoudleID { get; set; }

        /// <summary>
        /// 是否有考试
        /// </summary>
        public bool IsExam { get; set; }

        /// <summary>
        /// 是否可评论
        /// </summary>
        public bool IsComment { get; set; }

        public string PraiseOrProgress { get; set; }
        /// <summary>
        /// 学习可得积分
        /// </summary>
        public decimal MaxStudyIntegral { get; set; }
        /// <summary>
        /// 考试可得积分
        /// </summary>
        public decimal MaxExamIntegral { get; set; }
    }

    public class CourseStatistics
    {
        public string ID { get; set; }
        public string ModuleID { get; set; }
        public string ProjID { get; set; }
        public string Proj2ID { get; set; }
        public int ScoreAvg { get; set; }
        public int ScoreSum { get; set; }
        /// <summary>
        /// 评论数
        /// </summary>
        public int CommentSum { get; set; }
        /// <summary>
        /// 点赞数
        /// </summary>
        public int PraiseSum { get; set; }
    }
}
