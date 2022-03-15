using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;

namespace LearningFucker.Models
{
    public enum StudyStatus
    {
        Stop,
        Processing,
        Completed
    }

    public class Study
    {
        /// <summary>
        /// 学习编号
        /// </summary>
        public string LogId { get; set; }
        public string ProjID { get; set; }
        public string ProjType { get; set; }
        public string Proj2ID { get; set; }
        public string WareID { get; set; }
        /// <summary>
        /// 本周总学习时长(分钟)
        /// </summary>
        public decimal SumStudyTime { get; set; }
        /// <summary>
        /// 课程已学习时长
        /// </summary>
        public decimal SumStudyTimew { get; set; }
        /// <summary>
        /// 本周总学习积分
        /// </summary>
        public decimal SumIntegral { get; set; }
        /// <summary>
        /// 今天学习时长(分钟)
        /// </summary>
        public decimal TodayStudyTime { get; set; }
        /// <summary>
        /// 本周考试已获取积分
        /// </summary>
        public decimal ExamIntegral { get; set; }

        /// <summary>
        /// 每周可获取考试积分
        /// </summary>
        public decimal ExamMaxIntegral { get; set; }

        /// <summary>
        /// 每周可学习积分
        /// </summary>
        public decimal MaxIntegral { get; set; }
        /// <summary>
        /// 学习获取积分
        /// </summary>
        public decimal StudyIntegral { get; set; }

        /// <summary>
        /// 课程还允许获取的积分
        /// </summary>
        public decimal AllowIntegral { get; set; }

        public CourseDetail Course { get; set; }

        public CourseAppendix Appendix { get; set; }
        public WareDetail Ware { get; set; }

        public Action<Study> StudyComplete;

        /// <summary>
        /// 开始学习时的积分
        /// </summary>
        public decimal InitIntegral { get; set; }

        public int StudyDuration { get; internal set; }

        private void AddStudyDuration(int duration)
        {
            this.StudyDuration = StudyDuration + duration;
            this.Course.StudyDuration = this.Course.StudyDuration + duration;
            this.Ware.StudyDuration = this.Ware.StudyDuration + duration;
        }

        private Fucker fucker;

        public StudyStatus Status { get; internal set; }

        public async System.Threading.Tasks.Task Start(Fucker fucker, CancellationToken token)
        {
            this.fucker = fucker;
            if (Status == StudyStatus.Completed)
                return;

            this.Status = StudyStatus.Processing;

            await Run(token);
        }

        private async System.Threading.Tasks.Task Run(CancellationToken token)
        {
            try
            {
                while(true)
                {
                    if (token.IsCancellationRequested)
                    {
                        this.Complete();
                        break;
                    }

                    await this.SaveStudyInfo();


                    await System.Threading.Tasks.Task.Delay(Fucker.POLLING_TIME);
                    await this.GetStudyInfo();
                    if (this.Status == StudyStatus.Completed)
                        return;
                    if (this.Status == StudyStatus.Stop)
                        return;

                    
                    
                    AddStudyDuration(Fucker.POLLING_TIME / 1000);
                }
                
            }
            catch (Exception ex)
            {
                fucker.Worker.ReportError(ex.Message);                
            }
        }


        public void Complete()
        {
            Status = StudyStatus.Completed;
            StudyComplete?.Invoke(this);
        }

        public async System.Threading.Tasks.Task GetStudyInfo()
        {
            await fucker.GetStudyInfo(this);
            await fucker.GetWareIntegral(this);
            if(this.AllowIntegral == 0.0m)
            {
                this.Complete();
            }
            
        }

        private async System.Threading.Tasks.Task SaveStudyInfo()
        {
            await fucker.SaveStudyLog(this);
        }

        
    }
}
