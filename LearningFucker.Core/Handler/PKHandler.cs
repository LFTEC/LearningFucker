using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using LearningFucker.Models;
using System.Timers;

namespace LearningFucker.Handler
{
    public class PKHandler : TaskHandlerBase
    {

        public PKHandler(CancellationToken token, Models.Task task)
            :base(token, task)
        {

        }

        private const int TMNUMBER = 5;
        private Arena Arena { get; set; }

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
                    

                while (true)
                {
                    if (CancellationToken.IsCancellationRequested)
                    {
                        this.Stop();
                        return;
                    }

                    Arena = await Fucker.GetArena();
                    Arena.TmNumber = TMNUMBER;
                    if (await Fucker.JoinArena(Arena) && Arena.GroupId != "")
                    {
                        await GetArenaBothSide();
                        await StartRound(0);
                    }
                    else
                        await System.Threading.Tasks.Task.Delay(1000);
                    if (this.LimitIntegral <= this.Integral)
                    {
                        Complete();
                        return;
                    }
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



        private async System.Threading.Tasks.Task StartRound(int roundIndex)
        {
            try
            {
                var round = await Fucker.Fight(Arena);

                if (round.CurrentIndex == roundIndex && round.Status == "Start")
                {
                    if (Arena.Rounds == null)
                        Arena.Rounds = new List<Round>();

                    Arena.Rounds.Add(round);

                    var interval = new Random().Next(3000, 6000);

                    await System.Threading.Tasks.Task.Delay(interval);
                    await OnReply(roundIndex);
                    
                }
                else if (roundIndex == Arena.TmNumber)
                {
                    if (round.CurrentIndex == 0)
                    {

                        await Fucker.EndFight();
                        if (await Fucker.GetPKResult(Arena))
                        {
                            var gladiator = Arena.Results.FindIndex(s => s.Gladiator.UserName == Fucker.Worker.User.UserName);
                            await Fucker.GetIntegralDetail(Arena.Results[gladiator]);
                            this.Integral += Arena.Results[gladiator].PKScroe;

                        }
                    }
                    else
                    {
                        await System.Threading.Tasks.Task.Delay(1000);
                        await StartRound(roundIndex);
                    }
                }
                else
                {
                    await System.Threading.Tasks.Task.Delay(1000);
                    await StartRound (roundIndex);
                }
            }
            catch(Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Stop();
            }

            
        }

        private async System.Threading.Tasks.Task OnReply(int roundIndex)
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
                    await StartRound (++roundIndex);

                }
            }
            catch(Exception ex)
            {
                Fucker.Worker.ReportError(ex.Message);
                Stop();
            }
        }

    }
}
