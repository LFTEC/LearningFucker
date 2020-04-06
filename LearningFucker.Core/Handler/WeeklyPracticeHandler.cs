using System;
using System.Collections.Generic;
using System.Text;
using LearningFucker.Models;

namespace LearningFucker.Handler
{
    public class WeeklyPracticeHandler : TaskHandlerBase
    {
        public override bool Start(Fucker fucker)
        {
            if (!base.Start(fucker))
            {
                fucker.Worker.Say("每周一练启动失败!");
                return false;
            }

            if (TaskForWork.LimitIntegral <= TaskForWork.Integral)
            {
                Complete();
                return true;
            }

            Start();
            return true;
        }

        private void Start()
        {
            try
            {
                DoWork();
            }
            catch (TransportException ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Fucker.Worker.Say("每日一练启动失败!");
                Stop();
            }
            catch (Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Fucker.Worker.Say("每日一练启动失败!");
                Stop();
            }
        }

        public async override void DoWork()
        {
            try
            {
                List<ExerciseAnswer> answers = new List<ExerciseAnswer>();
                var practiceQuestionList = await Fucker.StartWeeklyPractice();
                foreach (var question in practiceQuestionList.Questions)
                {
                    answers.Add(new ExerciseAnswer() { TmID = question.TmID, AnswerContent = question.Answers.Replace(";", ",") });
                }
                await Fucker.HandIn(practiceQuestionList, answers, 15);
                await Fucker.GetResult(practiceQuestionList.Result);

                TaskForWork.Integral += practiceQuestionList.Result.Integral;

                if (TaskForWork.LimitIntegral <= TaskForWork.Integral)
                {
                    this.Complete();
                    return;
                }
                else
                    Start();
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
    }
}
