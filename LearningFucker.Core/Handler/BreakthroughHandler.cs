using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningFucker.Models;

namespace LearningFucker.Handler
{
    public class BreakthroughHandler : TaskHandlerBase
    {
        private BreakthroughList breakthroughList;

        public override bool Start(Fucker fucker)
        {
            if (!base.Start(fucker))
            {
                fucker.Worker.Say("闯关答题启动失败!");
                return false;
            }

            if(TaskForWork.LimitIntegral <= TaskForWork.Integral)
            {
                Complete();
                return true;
            }

            Start();
            return true;

        }

        private async void Start()
        {
            try
            {
                breakthroughList = await Fucker.GetBreakthroughList();
                DoWork();
            }
            catch (TransportException ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Fucker.Worker.Say("闯关答题启动失败!");
                Stop();
            }
            catch (Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Fucker.Worker.Say("闯关答题启动失败!");
                Stop();
            }
        }

        public async override void DoWork()
        {
            try
            {
                List<ExerciseAnswer> answers;
                foreach (var item in breakthroughList.List)
                {
                    answers = new List<ExerciseAnswer>();
                    if (!item.IsPass)
                    {
                        await Fucker.StartBreakthrough(item);
                        foreach (var question in item.Questions)
                        {
                            answers.Add(new ExerciseAnswer() { TmID = question.TmID, AnswerContent = question.Answers.Replace(";", ",") });
                        }
                        await Fucker.HandIn(item, answers, 15);
                        await Fucker.GetResult(item.Result);

                        TaskForWork.Integral += item.Result.Integral;

                        if (TaskForWork.LimitIntegral <= TaskForWork.Integral)
                        {
                            this.Complete();
                            return;
                        }
                    }
                }

                if (TaskForWork.LimitIntegral > TaskForWork.Integral)
                {
                    Start();
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

    }
}
