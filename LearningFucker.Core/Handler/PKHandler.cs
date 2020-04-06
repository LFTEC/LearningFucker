using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningFucker.Models;
using System.Timers;

namespace LearningFucker.Handler
{
    public class PKHandler : TaskHandlerBase
    {




        private Arena Arena { get; set; }

        public override bool Start(Fucker fucker)
        {
            if (!base.Start(fucker)) return false;

            Start();
            return true;
        }

        private void Start()
        {            
            DoWork();
        }

        public async override void DoWork()
        {
            try
            {
                if (this.TaskStatus == TaskStatus.Stopping)
                {
                    TaskStatus = TaskStatus.Stopped;
                    TaskForWork.TaskStatus = TaskStatus.Stopped;
                    return;
                }

                Arena = await Fucker.GetArena();
                if (Arena == null) throw new Exception("error");

                if (await Fucker.JoinArena(Arena) && Arena.GroupId != "")
                {
                    await GetArenaBothSide();
                    StartRound(0);
                }
                else
                {
                    DoWork();
                }
            }
            catch(Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Stop();
            }
        }

        private async System.Threading.Tasks.Task GetArenaBothSide()
        {
            if (await Fucker.GetArenaBothSide(Arena))
            {
                await Fucker.ReadyToFight();
                return;
            }
            else
                await GetArenaBothSide();
        }



        private async void StartRound(int roundIndex)
        {
            try
            {
                var round = await Fucker.Fight(Arena);
                if (round == null)
                {
                    throw new Exception("error");

                }

                if (round.CurrentIndex == roundIndex && round.Status == "Start")
                {
                    if (Arena.Rounds == null)
                        Arena.Rounds = new List<Round>();

                    Arena.Rounds.Add(round);

                    Timer timer = new Timer();
                    timer.Interval = new Random().Next(5000, 9000);
                    timer.AutoReset = false;
                    timer.Elapsed += new ElapsedEventHandler((s, e) => OnReply(s, e, roundIndex));
                    timer.Start();
                }
                else if (roundIndex == Arena.TmNumber)
                {
                    if (round.CurrentIndex == 0)
                    {

                        await Fucker.EndFight();
                        if (await Fucker.GetPKResult(Arena))
                        {
                            var gladiator = Arena.Results.FindIndex(s => s.Gladiator.UserName == Fucker.Worker.User.UserName);
                            if (gladiator < 0) throw new Exception("error");

                            TaskForWork.Integral += Arena.Results[gladiator].PKScroe;
                            if (TaskForWork.LimitIntegral == TaskForWork.Integral)
                                Complete();
                            else
                                DoWork();
                        }
                    }
                    else
                        StartRound(roundIndex);
                }
                else
                {
                    StartRound(roundIndex);
                }
            }
            catch(Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Stop();
            }

            
        }

        private async void OnReply(object sender, ElapsedEventArgs args, int roundIndex)
        {
            try
            {
                var round = Arena.Rounds[roundIndex];
                var myself = Arena.BothSides.FindIndex(s => s.Username == Fucker.Worker.User.UserName);
                var rival = myself ^ 1;

                var myRight = round.AnswerResult[myself].Where(s => s == 1).Count();
                var rivalRight = round.AnswerResult[rival].Where(s => s == 1).Count();

                string answer;
                if (myRight >= rivalRight + 2)
                {
                    answer = round.Question.Answers == "A" ? "B" : "A";
                }
                else
                    answer = round.Question.Answers;

                if (await Fucker.SubmitQuestion(Arena, round, answer.Replace(";", ",")))
                {
                    StartRound(++roundIndex);

                }
            }
            catch(Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Stop();
            }
        }

        public override bool Stop()
        {
            if (TaskStatus == TaskStatus.Working)
            {
                TaskStatus = TaskStatus.Stopping;
                return true;
            }
            else
                return false;
        }

        protected override bool Complete()
        {
            TaskStatus = TaskStatus.Completed;
            TaskForWork.TaskStatus = TaskStatus.Completed;
            return true;
        }
    }
}
