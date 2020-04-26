using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LearningFucker.CLI.Commands
{
    public interface ICommand
    {
        public Task<bool> Execute(string[] args);
    }
}
