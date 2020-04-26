using System;
using System.Configuration;

namespace LearningFucker.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Cli cli = new Cli(args);
            cli.Deal().Wait();
        }

        public static string ReadInfo(string prompt)
        {
            System.Console.Write(prompt);
            return System.Console.ReadLine();
        }
    }
}
