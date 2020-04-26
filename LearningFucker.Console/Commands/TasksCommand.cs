using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LearningFucker.Models;
using LearningFucker;
using System.Linq;

namespace LearningFucker.CLI.Commands
{
    public class TasksCommand : ICommand
    {
        public async Task<bool> Execute(string[] args)
        {
            string option;
            if (args.Length <= 1)
                option = "list";
            else
                option = args[1];

            switch (option)
            {
                case "list":
                    var taskList = await GetTaskList();
                    ShowTaskList(taskList);
                    return true;
                case "id":
                    var param = args.Skip(2).ToArray();
                    var task = await GetTask(param);
                    ShowTask(task);
                    return true;
                default:
                    Console.WriteLine(string.Format(Properties.Resources.unrecognize_option, option));
                    return false;
            }
        }

        public async Task<TaskList> GetTaskList()
        {
            var user = ConfigurationManager.GetDefaultUser();
            Fucker fucker = new Fucker();
            var userInfo = await fucker.Login(user.UserId, user.Password);
            if(userInfo == null)
            {
                ConfigurationManager.ResetDefaultUser();
                return await GetTaskList();                
            }


            return await fucker.GetTaskList("0");
        }

        public async Task<LearningFucker.Models.Task> GetTask(string[] args)
        {
            int id;
            if(args == null || args.Length == 0 || !int.TryParse(args[0], out id) )
            {
                Console.WriteLine(string.Format(Properties.Resources.must_input_id));
                Console.WriteLine(Properties.Resources.for_help);
                return null;
            }

            var taskList = await GetTaskList();
            var task = taskList.List.FirstOrDefault(s => s.TaskType == id);
            return task;
        }

        public void ShowTaskList(TaskList list)
        {
            Console.WriteLine("task list: " );
            foreach (var item in list.List)
            {
                Console.Write(item.TaskType);
                Console.Write('\t');
                Console.Write(item.Name);
                Console.Write('\t');
                Console.Write(item.LimitIntegral);
                Console.WriteLine();
            }
            Console.WriteLine(string.Format(Properties.Resources.total_lines, list.Count));
        }

        public void ShowTask(Models.Task task)
        {
            if(task == null)
            {
                Console.WriteLine(string.Format(Properties.Resources.must_input_id));
                Console.WriteLine(Properties.Resources.for_help);
                return ;
            }

            Console.Write("id:\t");
            Console.WriteLine(task.TaskType);
            Console.Write("name:\t");
            Console.WriteLine(task.Name);
            Console.Write("explain:\t");
            Console.WriteLine(task.Explain);
            Console.Write("integral:\t");
            Console.WriteLine(task.LimitIntegral);
            Console.Write("enabled:\t");
            Console.WriteLine(task.Enabled);
            Console.Write("hidden:\t");
            Console.WriteLine(task.IsHidden);
            Console.Write("period:\t");
            Console.WriteLine(task.Period);
        }
    }
}
