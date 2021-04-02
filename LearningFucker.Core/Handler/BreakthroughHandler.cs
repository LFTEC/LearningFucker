using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using LearningFucker.Models;

namespace LearningFucker.Handler
{
    public class BreakthroughHandler : TaskHandlerBase
    {
        private BreakthroughList breakthroughList;

        public BreakthroughHandler(CancellationToken token, Models.Task task)
            :base(token, task)
        {

        }

        protected override async Task<bool> Start()
        {
            this.LimitIntegral = this.Task.LimitIntegral;
            this.Integral = this.Integral;


            breakthroughList = await Fucker.GetBreakthroughList();
            return true;
        }


        public async override System.Threading.Tasks.Task DoWork()
        {
            try
            {
                List<ExerciseAnswer> answers;

                if (LimitIntegral <= Integral)
                {
                    Complete();
                    return;
                }

                DataContext dataContext = new DataContext();

                while(true)
                {
                    breakthroughList = await Fucker.GetBreakthroughList();

                    if(!breakthroughList.List.Any(s=>s.CanJoin))
                    {
                        Stop();
                        break;
                    }

                    if (this.CancellationToken.IsCancellationRequested)
                    {
                        Stop();
                        return;
                    }

                    foreach (var item in breakthroughList.List.Where(s=>s.CanJoin))
                    {   
                        answers = new List<ExerciseAnswer>();
                        await Fucker.StartBreakthrough(item);
                        foreach (var question in item.Questions)
                        {
                            var q = await dataContext.GetRow(question.TmID);
                            answers.Add(new ExerciseAnswer() { TmID = question.TmID, AnswerContent = q == null ? "A" : q.Answers.Replace(";", ",") });
                        }
                        await Fucker.HandIn(item, answers, 15);
                        await Fucker.GetResult(item.Result);
                        await Fucker.GetIntegralDetail(item.Result);
                        await UpdateQuestionBank(item.Result);

                        this.Integral += item.Result.Integral;

                        if (this.LimitIntegral <= this.Integral)
                        {
                            this.Complete();
                            return;
                        }
                    }
                }               

            }
            catch (TransportException ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Fucker.Worker.Say("闯关答题答题失败!");
                Stop();
            }
            catch (Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Fucker.Worker.Say("闯关答题答题失败!");
                Stop();
            }
        }

        private async System.Threading.Tasks.Task UpdateQuestionBank(BreakthroughResult result)
        {
            try
            {
                DataContext dataContext = new DataContext();
                if (await Fucker.ReviewBreakthrough(result))
                {
                    foreach (var item in result.Questions)
                    {
                        if (await dataContext.GetRow(item.TmID) == null)
                        {
                            await dataContext.InsertRow(item);
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
