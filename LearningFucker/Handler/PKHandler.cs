using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningFucker.Models;

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

        private async void Start()
        {
            Arena = await Fucker.GetArena();
            DoWork();
        }

        public async override void DoWork()
        {
            if(await Fucker.JoinArena(Arena) && Arena.GroupId != "")
            {
                await GetArenaBothSide();

            }
            else
            {
                DoWork();
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

        private async void StartFight()
        {
            for (int i = 0; i < Arena.TmNumber; i++)
            {
                if(!await StartRound(i))
                {

                }
            }
        }

        private async Task<bool> StartRound(int roundIndex)
        {
            var round = await Fucker.Fight(Arena);
            if(round == null)
            {
                throw new Exception("error");              

            }

            if (round.CurrentIndex == roundIndex && round.Status == "start")
            {
                if (Arena.Rounds == null)
                    Arena.Rounds = new List<Round>();

                Arena.Rounds.Add(round);

            }
            else
            {
                return await StartRound(roundIndex);
            }
           

            
        }

        public override bool Stop()
        {
            throw new NotImplementedException();
        }

        protected override bool Complete()
        {
            throw new NotImplementedException();
        }
    }
}
