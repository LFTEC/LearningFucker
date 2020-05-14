using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LearningFucker.Models
{
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
        /// 总学习时长(分钟)
        /// </summary>
        public decimal SumStudyTime { get; set; }
        /// <summary>
        /// 今天学习时长(分钟)
        /// </summary>
        public decimal TodayStudyTime { get; set; }
        /// <summary>
        /// 考试获取积分
        /// </summary>
        public decimal ExamIntegral { get; set; }
        /// <summary>
        /// 学习获取积分
        /// </summary>
        public decimal StudyIntegral { get; set; }
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

        private Timer timer;
        private Fucker fucker;

        public bool Complete { get; internal set; }

        public void Start(Fucker fucker)
        {
            this.fucker = fucker;
            this.Complete = false;
            if (timer == null)
            {
                timer = new Timer(Fucker.POLLING_TIME);
                timer.Elapsed += Timer_Elapsed;
            }

            if (timer.Enabled)
                timer.Stop();
            timer.Start();
        }

        public void Stop()
        {
            if (timer != null && timer.Enabled)
            {
                timer.Stop();
            }

            this.Complete = true;
            StudyComplete?.Invoke(this);

        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                await fucker.SaveStudyLog(this);
                await fucker.GetStudyInfo(this);
                AddStudyDuration(Fucker.POLLING_TIME / 1000);

                if (this.Ware.StudyDuration >= this.Ware.Duration)
                {
                    Ware.Complete = true;

                    this.Stop();
                }

                if (this.Course.StudyDuration >= this.Course.TotalMinute)
                {
                    Course.Complete = true;
                    this.Stop();
                }

                //if(this.TodayStudyTime * 60 >= this.Course.TotalMinute)
                //{
                //    this.Complete = true;
                //    Course.Complete = true;
                //    this.Stop();
                //}

                //if(this.Course.StudyDuration - this.TodayStudyTime * 60 >= 60)
                //{
                //    this.Complete = true;
                //    Course.Complete = true;
                //    this.Stop();
                //}

                //如果已学习积分达到课程允许学到的积分, 则完成学习
                if(this.StudyIntegral - this.InitIntegral >= this.Ware.AllowIntegral)
                {
                    Ware.Complete = true;
                    this.Stop();
                }

                //学习时长增加而积分不增加时, 停止学习
                if (this.StudyDuration / 120.0m - (this.StudyIntegral - this.InitIntegral) >= 1.0m)
                {
                    Course.Complete = true;
                    this.Stop();
                }

                if (this.StudyIntegral == this.Appendix.MaxStudyIntegral)
                {

                    Course.Complete = true;
                    this.Stop();
                }
            }
            catch(Exception ex)
            {
                fucker.Worker.ReportError(ex.Message);
                this.Stop();
            }

        }
    }
}
