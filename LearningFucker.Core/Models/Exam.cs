using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LearningFucker.Models
{
    public class ExamList
    {
        /// <summary>
        /// 考卷清单数量
        /// </summary>
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
        /// <summary>
        /// 清单
        /// </summary>
        [JsonProperty(PropertyName = "list")]
        public List<Exam> List { get; set; }

        public string ProjID { get; set; }
    }

    public class Exam
    {
        public string ExamID { get; set; }
        public string ExamName { get; set; }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int ExamOrder { get; set; }
        /// <summary>
        /// 考试类型(0:随机)
        /// </summary>
        public int ExamType { get; set; }
        /// <summary>
        /// 考试时间(分钟)
        /// </summary>
        public int ExamTime { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public string ProjID { get; set; }

        public ExamDetail ExamDetail { get; set; }

        /// <summary>
        /// 试卷
        /// </summary>
        public List<Paper> Papers { get; set; }
    }

    public class ExamDetail
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 考试总分
        /// </summary>
        public decimal TotalScore { get; set; }
        /// <summary>
        /// 允许考试次数
        /// </summary>
        public int MaxExamCount { get; set; }
        /// <summary>
        /// 试题数量
        /// </summary>
        public int QuestionCount { get; set; }
        /// <summary>
        /// 考生名称
        /// </summary>
        public string UserRealname { get; set; }
        /// <summary>
        /// 考试次数
        /// </summary>
        public int JoinCount { get; set; }
        /// <summary>
        /// 是否允许考试
        /// </summary>
        public bool AllowExam { get; set; }
        /// <summary>
        /// 不允许原因
        /// </summary>
        public string Reason { get; set; }
    }

    /// <summary>
    /// 考试卷
    /// </summary>
    public class Paper
    {
        [JsonProperty(PropertyName = "CacheId")]
        public string PaperId { get; set; }

        [JsonProperty(PropertyName = "Exam")]
        public PaperSetting Setting { get; set; }

        /// <summary>
        /// 答题时间(秒)
        /// </summary>
        public int Remain { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string ServerTime { get; set; }

        /// <summary>
        /// 试题清单
        /// </summary>
        public List<Question> Questions { get; set; }

        /// <summary>
        /// 阅卷结果
        /// </summary>
        public Result Result { get; set; }
    }

    /// <summary>
    /// 练习卷
    /// </summary>
    public class Exercise
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("list")]
        public List<Question> Questions { get; set; }

        public ExerciseResult Result { get; set; }
    }

    public class PaperSetting
    {
        /// <summary>
        /// 试卷名称
        /// </summary>
        public string ExamName { get; set; }
        /// <summary>
        /// 多选模式
        /// </summary>
        public int MultiOptionMode { get; set; }
        /// <summary>
        /// 试题类型
        /// </summary>
        public int[] QuestionTypes { get; set; }
        /// <summary>
        /// 选择题选项随机显示
        /// </summary>
        public bool IsOptionRandom { get; set; }
        /// <summary>
        /// 题目随机显示
        /// </summary>
        public bool IsQuestionRandom { get; set; }
    }

    public class Question
    {
        public int TmSourceType { get; set; }

        /// <summary>
        /// 题库编号
        /// </summary>
        [JsonProperty(PropertyName = "Tk_ID")]
        public int TkID { get; set; }

        /// <summary>
        /// 题目编号
        /// </summary>
        [JsonProperty("Tm_ID")]
        public int TmID { get; set; }


        /// <summary>
        /// 题型
        /// </summary>
        [JsonProperty("Tm_Tx")]
        public int TmTx { get; set; }

        /// <summary>
        /// 题型归类
        /// </summary>
        [JsonProperty("Tm_BaseTx")]
        public string TmBaseTx { get; set; }

        /// <summary>
        /// 题型描述
        /// </summary>
        [JsonProperty("Tm_Tx_Str")]
        public string TmTxStr { get; set; }

        /// <summary>
        /// 题目
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 题目索引
        /// </summary>
        [JsonProperty("Tm_Key")]
        public string TmKey { get; set; }

        /// <summary>
        /// 选项
        /// </summary>
        public string Options { get; set; }

        /// <summary>
        /// 答案(多选分号隔开, 答案都是ABCD)
        /// </summary>
        public string Answers { get; set; }

        /// <summary>
        /// 难度
        /// </summary>
        public string Difficulty { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 考生提交答案(同答案)
        /// </summary>
        public string UserAnswer { get; set; }

        /// <summary>
        /// 回答是否正确(0:正确, -1:不正确)
        /// </summary>
        public int AnswerRight { get; set; }

        /// <summary>
        /// 考生得分
        /// </summary>
        public decimal UserScore { get; set; }

        /// <summary>
        /// 题目分数
        /// </summary>
        public decimal Score { get; set; }
    }

    public class Result
    {
        public string ResultId { get; set; }

        /// <summary>
        /// 是否允许查看试卷
        /// </summary>
        public bool AllowSeePaper { get; set; }


        /// <summary>
        /// 是否允许查看成绩
        /// </summary>
        public bool AllowSeeScore { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// 得分
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// 答卷时间(分钟)
        /// </summary>
        public int ElapsedMinutes { get; set; }

        /// <summary>
        /// 错题数
        /// </summary>
        public int ErrorQuestionCount { get; set; }

        /// <summary>
        /// 获得积分
        /// </summary>
        public decimal Integral { get; set; }

        public List<Question> Questions { get; set; }
    }

    public class ExerciseResult
    {
        public string ResultId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 开始练习时间
        /// </summary>
        public string AddTime { get; set; }

        /// <summary>
        /// 结束练习时间
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 答对数量
        /// </summary>
        public string RightCount { get; set; }

        /// <summary>
        /// 答错数量
        /// </summary>
        public string WrongCount { get; set; }

        /// <summary>
        /// 获得积分
        /// </summary>
        public decimal Integral { get; set; }

        /// <summary>
        /// 正确答案
        /// </summary>
        [JsonProperty("list")]
        public List<Question> Questions { get; set; }
    }

    public class BreakthroughResult
    {
        [JsonProperty("resultId")]
        public string ResultId { get; set; }

        /// <summary>
        /// 状态 ,现在是 Pass是成功
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 开始答题时间
        /// </summary>
        public string BTime { get; set; }
        /// <summary>
        /// 结束答题时间
        /// </summary>
        public string ETime { get; set; }

        /// <summary>
        /// 答题时间
        /// </summary>
        public int UseSec { get; set; }

        /// <summary>
        /// 得分
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// 是否通过
        /// </summary>
        public int IsPass { get; set; }


        /// <summary>
        /// 题目数量
        /// </summary>
        public int TotalNum { get; set; }


        /// <summary>
        /// 正确数量
        /// </summary>
        public int RightNum { get; set; }

        /// <summary>
        /// 答案
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// 获得积分
        /// </summary>
        public decimal Integral { get; set; }

        /// <summary>
        /// 正确答案
        /// </summary>
        [JsonProperty("list")]
        public List<Question> Questions { get; set; }
    }

    public class Answer
    {
        [JsonProperty("tmid")]
        public int TmID { get; set; }

        /// <summary>
        /// 答案(逗号分隔)
        /// </summary>
        [JsonProperty("answer")]
        public string AnswerContent { get; set; }

        [JsonProperty("answerfile")]
        public string AnswerFile { get; set; }

        [JsonProperty("score")]
        public string Score { get; set; }
    }

    public class ExerciseAnswer
    {
        [JsonProperty("tmid")]
        public int TmID { get; set; }

        /// <summary>
        /// 答案(逗号分隔)
        /// </summary>
        [JsonProperty("answer")]
        public string AnswerContent { get; set; }
    }
}
