using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LearningFucker.CLI.Commands;
using LearningFucker.CLI.Properties;
namespace LearningFucker.CLI
{
    public class Cli
    {
        private string[] args;
        public Cli(string[] args)
        {
            this.args = args;
        }

        public async Task Deal()
        {
            if (args == null || args.Length == 0)
            {
                System.Console.WriteLine("please type correct command!");
                return;
            }
            ICommand cmd = null;
            string command = args[0];

            switch (command)
            {   
                case "--tasks":
                case "-T":
                    cmd = new TasksCommand();
                    await cmd.Execute(args);
                    break;
                default:
                    System.Console.WriteLine(string.Format(Resources.unrecognize_option, command));
                    System.Console.WriteLine(Resources.for_help);
                    break;
            }
        }
    }



    public class UserInfo
    {
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}
