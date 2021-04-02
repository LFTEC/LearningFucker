using System;
using System.Collections.Generic;
using System.Text;
using LearningFucker.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace LearningFucker.Handler
{
    public class WeeklyPracticeHandler : TaskHandlerBase
    {

        public WeeklyPracticeHandler(CancellationToken token, Models.Task task)
            :base(token, task)
        {

        }

        protected override Task<bool> Start()
        {
            this.LimitIntegral = this.Task.LimitIntegral;
            this.Integral = this.Task.Integral;
            return System.Threading.Tasks.Task.FromResult(true);
        }


        public async override System.Threading.Tasks.Task DoWork()
        {
            try
            {
                if (this.LimitIntegral <= this.Integral)
                {
                    Complete();
                    return;
                }

                DataContext dataContext = new DataContext();

                while (true)
                {
                    if(CancellationToken.IsCancellationRequested)
                    {
                        this.Stop();
                        return;
                    }

                    var practiceList = await Fucker.GetWeeklyPracticeList();
                    var list = practiceList?.list[0];
                    var week = list?.WeekList?.FirstOrDefault(s => s.State == "Doing");
                    if(week == null)
                    {
                        continue;
                    }

                    List<ExerciseAnswer> answers = new List<ExerciseAnswer>();
                    var practiceQuestionList = await Fucker.StartWeeklyPractice(week);
                    foreach (var question in practiceQuestionList.Questions)
                    {
                        var item = await dataContext.GetRow(question.TmID);
                        answers.Add(new ExerciseAnswer() { TmID = question.TmID, AnswerContent = item == null? "A": item.Answers.Replace(";", ",") });
                    }
                    await Fucker.HandIn(practiceQuestionList, answers, 15, week);
                    await Fucker.GetResult(practiceQuestionList.Result);
                    await Fucker.ReviewResult(practiceQuestionList.Result);
                    await UpdateQuestionBank(practiceQuestionList.Result);

                    this.Integral += practiceQuestionList.Result.Integration;

                    if (this.LimitIntegral <= this.Integral)
                    {
                        Complete();
                        return;
                    }
                }
            }
            catch (TransportException ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Fucker.Worker.Say("每日一练答题失败!");
                Stop();
            }
            catch (Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Fucker.Worker.Say("每日一练答题失败!");
                Stop();
            }
        }

        private async System.Threading.Tasks.Task UpdateQuestionBank(PracticeResult result)
        {
            try
            {
                DataContext dataContext = new DataContext();

                foreach (var item in result.Questions)
                {
                    if (await dataContext.GetRow(item.TmID) == null)
                    {
                        await dataContext.InsertRow(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
            }
        }
    }
}
