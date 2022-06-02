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

                var practiceList = await Fucker.GetWeeklyPracticeList();
                var list = practiceList?.list[0];
                var week = list?.WeekList?.FirstOrDefault(s => s.State == "Doing");
                if (week == null)
                {
                    return;
                }

                while (true)
                {
                    if(CancellationToken.IsCancellationRequested)
                    {
                        this.Stop();
                        return;
                    }                    

                    List<ExerciseAnswer> answers = new List<ExerciseAnswer>();
                    var practiceQuestionList = await Fucker.StartWeeklyPractice(week);
                    foreach (var question in practiceQuestionList.Questions)
                    {
                        var item = await dataContext.GetRow(question.TmID);
                        answers.Add(new ExerciseAnswer() { TmID = question.TmID, AnswerContent = item == null? "A": item.Answers.Replace(";", ",") });
                    }
                    await Fucker.HandIn(practiceQuestionList, answers, 15, week);
                    await System.Threading.Tasks.Task.Delay(1000);
                    await Fucker.GetResult(practiceQuestionList.Result);
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
                Fucker.Worker.Say("每周一练答题失败!");
                Stop();
            }
            catch (Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Fucker.Worker.Say("每周一练答题失败!");
                Stop();
            }
        }

        private async System.Threading.Tasks.Task UpdateQuestionBank(PracticeResult result)
        {
            try
            {
                DataContext dataContext = new DataContext();

                if (await Fucker.ReviewResult(result))
                {
                    foreach (var item in result.Questions)
                    {
                        var row = await dataContext.GetRow(item.TmID);
                        if (row == null)
                        {
                            await dataContext.InsertRow(item);
                        }
                        else if(row.Answers != item.Answers)
                        {
                            await dataContext.UpdateRow(item);
                        }
                        
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
