using System;
using CommandLine;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using ConsoleTables;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace LearningFucker.Console
{
    class Program
    {
        
        static void Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://server.jcdev.cc:9200"))
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6
                    })
                    .Enrich.WithProperty("guid", System.Guid.NewGuid())
                    .CreateLogger();



                Settings dealer = new Settings();

                dealer.InitConfig();

                Parser.Default.ParseArguments<AddUser, List, Study, Learn>(args)
                    .WithParsed<AddUser>(options =>
                    {
                        var userId = options.UserName;
                        var password = options.Password;
                        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
                        {
                            System.Console.WriteLine("username or password incorrect!");
                            return;
                        }
                        else
                        {
                            userId = userId.Trim();
                            password = password.Trim();
                            Job job = new Job(userId, password);
                            var login = job.LoginAsync().Result;
                            if (login)
                            {
                                dealer.SavePassword(userId, password);

                            //await dealer.Worker.Init();

                            System.Console.WriteLine($"add user: {userId} succeed!");

                            //var taskList = new List<int>();
                            //taskList.Add(14);
                            //Worker.StartWork(taskList, false);
                        }
                            else
                            {
                                System.Console.WriteLine("username or password incorrect!");
                            }
                        }
                    })
                    .WithParsed<RemoveUser>(options =>
                    {

                    })
                    .WithParsed<List>(async options =>
                    {
                        if (JobList == null) GetJobList().Wait();
                        if (JobList == null)
                            return;

                        if (options.UserStatus)  //显示用户信息
                        {
                            foreach (var job in JobList)
                            {
                                job.GetStatus().Wait();
                                System.Console.WriteLine($"user {job.Worker.User.RealName}({job.Worker.User.UserName}), dept {job.Worker.User.CompanyName}\r\nstatistics:");

                                System.Console.Write($"Ranking:{job.Worker.UserStatistics.IntegralRanking}");
                                System.Console.Write($"\tSum Integral:{job.Worker.UserStatistics.SumIntegral}");
                                System.Console.Write($"\tToday Integral:{job.Worker.UserStatistics.TodayIntegral}");
                                System.Console.Write($"\tWeek Integral:{job.Worker.UserStatistics.WeekIntegral}");
                                System.Console.Write($"\r\n");

                            }
                        }
                        else if (options.Tasks)  //显示用户任务清单
                        {
                            foreach (var job in JobList)
                            {
                                System.Console.WriteLine($"user {job.Worker.User.RealName}({job.Worker.User.UserName}), dept {job.Worker.User.CompanyName}\r\nTasks:\r\n");
                                var table = new ConsoleTable("id", "name", "integral", "required");
                                job.GetStatus().Wait();

                                foreach (var task in job.Worker.TaskList)
                                {
                                    table.AddRow(task.TaskType, task.Name, task.Integral, task.LimitIntegral);
                                }
                                table.Write(Format.Minimal);

                                System.Console.WriteLine();

                            }
                        }
                        else if (options.Courses) //显示必修课程清单
                        {
                            if (JobList.Count == 0)
                                System.Console.WriteLine("Need an user.");
                            else
                            {

                                foreach (var job in JobList)
                                {
                                    var table = new ConsoleTable("course id", "name");
                                    System.Console.WriteLine($"user {job.Worker.User.RealName}({job.Worker.User.UserName}), dept {job.Worker.User.CompanyName}\r\nCourses:");

                                    var courses = job.Worker.GetCourseList().Result;
                                    System.Console.WriteLine("Count of course: " + courses.Count().ToString());

                                    foreach (var course in courses)
                                    {
                                        table.AddRow(course.Item1, course.Item2);
                                    }
                                    table.Write(Format.Minimal);

                                    System.Console.WriteLine();
                                }                           
                            }
                        }
                    })
                    .WithParsed<Study>(options =>
                    {
                        GetJobList().Wait();
                        if (JobList == null) return;

                        List<Task> tasks = new List<Task>();
                        foreach (var job in JobList)
                        {
                            List<int> studyList;
                            if (options.AllTask || options.Tasks?.Count() == 0)
                                studyList = job.Worker.CanLearnedAsync().Result;
                            else
                            {
                                studyList = options.Tasks.ToList();
                                studyList = job.Worker.CanLearnedAsync(studyList).Result;
                            }

                            job.StudyList = studyList;
                            if (!(studyList?.Count > 0))
                                continue;

                            var task = job.Worker.StartWork(studyList, false);

                            tasks.Add(task);
                        //task.Start();


                        job.Worker.TaskRefresed += sender =>
                            {
                                BuildStudyTable();
                            };

                        }

                        BuildStudyTable();
                        Task.WaitAll(tasks.ToArray());
                    })
                    .WithParsed<Learn>(options =>
                    {
                        GetJobList().Wait();
                        if (JobList == null) return;

                        List<Task> tasks = new List<Task>();
                        foreach (var job in JobList)
                        {
                            var task = job.Worker.StartCourse(options.Course);                            
                            tasks.Add(task);
                            //task.Start();                            
                        }

                        Task.WaitAll(tasks.ToArray());
                    })
                    .WithNotParsed(error =>
                    {
                        error.ToList();
                    });
            }
            catch(Exception ex)
            {
                System.Console.Write(ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }



        static void BuildStudyTable()
        {
            var table = new ConsoleTable("user", "task", "required", "integral");

            foreach (var job in JobList)
            {
                
                foreach (var item in job.StudyList)
                {
                    var t = job.Worker.TaskList.FirstOrDefault(s => s.TaskType == item);
                    table.AddRow($"{job.Worker.User.RealName}({job.Worker.User.UserName})", t.Name, t.LimitIntegral, t.Integral);

                }
            }

            for (int i = System.Console.CursorTop; i >= 0; i--)
            {
                System.Console.CursorLeft = 0;
                System.Console.CursorTop = i;
                System.Console.Write(new string(' ', System.Console.BufferWidth));
            }

            table.Write(Format.Minimal);
        }

        static async Task GetJobList()
        {
            Settings dealer = new Settings();
            var users = dealer.ReadUsers();
            if(users.Count == 0)
            {
                System.Console.WriteLine("please add user first!");
                return;
            }
            JobList = new List<Job>();
            foreach (var user in users)
            {
                var job = new Job(user.name, user.password);
                if (await job.LoginAsync())
                {
                    JobList.Add(job);
                }
                else
                {
                    System.Console.WriteLine($"user {user.name} login failed!");
                }
            }
           
        }

        private static List<Job> JobList { get; set; }

    }

    public class Job
    {
        public Job(string user, string password)
        {
            this.user = user;
            this.password = password;
            Worker = new Worker();
        }


        private bool login;
        private string user;
        private string password;
        public Worker Worker { get; set; }
        public List<int> StudyList { get; set; }

        public async Task<bool> LoginAsync()
        {
            try
            {
                login = await Worker.Login(user, password);
                return login;
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task GetStatus()
        {
            try
            {
                if (!login)
                    System.Console.WriteLine($"user {user} not login!");
                await Worker.Init();
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }
    }

    [Verb("adduser", HelpText = "add user")]
    class AddUser
    {
        [Option('u', "user", HelpText = "user name", Required = true)]
        public string UserName { get; set; }
        [Option('p', "password", HelpText = "password", Required = true)]
        public string Password { get; set; }
    }
     
    [Verb("removeuser", HelpText = "remove user")]
    class RemoveUser
    {

    }

    [Verb("list", HelpText = "list what you want")]
    class List
    {
        [Option("tasks", HelpText = "list tasks", SetName = "task", Required = true)]
        public bool Tasks { get; set; }

        [Option("users", HelpText = "list user Information", SetName = "user", Required = true)]
        public bool UserStatus { get; set; }

        [Option("courses", HelpText = "course Information", SetName = "course", Required = true)]
        public bool Courses { get; set; }


    }

    [Verb("study", HelpText = "start study")]
    class Study
    {
        [Option("all", Default = false, SetName = "all")]
        public bool AllTask { get; set; }

        [Option("tasks", Separator = ';', HelpText = "tasks which you want to learn", SetName = "task")]
        public IEnumerable<int> Tasks { get; set; }
    }

    [Verb("learn", HelpText = "Learn specific course.")]
    class Learn
    {
        [Option('c', "course", HelpText = "course id", Required = true)]
        public string Course { get; set; }
    }

    public class Settings
    {
        public Settings()
        {
            Config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        public Configuration Config { get; set; }
        private const string KEY = "jcflRWUJqAs=";
        private const string IV = "hBoIpG2rhqE=";

        public void SavePassword(string userId, string password)
        {
            SymmetricAlgorithm sa = DES.Create();
            sa.Key = Convert.FromBase64String(KEY);
            sa.IV = Convert.FromBase64String(IV);
            byte[] content = Encoding.UTF8.GetBytes(password);

            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, sa.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(content, 0, content.Length);
            cs.FlushFinalBlock();
            var cryPassword = Convert.ToBase64String(ms.ToArray());

            var userSection = Config.GetSection("UserCredential") as UserSection;
            if (userSection.Users.Contain(userId))
                userSection.Users.GetUser(userId).Password = cryPassword;
            else
                userSection.Users.Add(userId, cryPassword);

            Config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("UserCredential");
        }

        public List<(string name,string password)> ReadUsers()
        {
            try
            {
                (string, string) data;
                List<(string, string)> list = new List<(string, string)>();
                var userSection = Config.GetSection("UserCredential") as UserSection;
                var Users = userSection?.Users;
                if (Users == null)
                    return null;

                SymmetricAlgorithm sa = DES.Create();
                sa.Key = Convert.FromBase64String(KEY);
                sa.IV = Convert.FromBase64String(IV);

                foreach (var user in Users.AllKeys)
                {
                    var User = Users.GetUser(user);

                    string cryPassword = User.Password;

                    byte[] content = Convert.FromBase64String(cryPassword);
                    var ms = new MemoryStream();
                    var cs = new CryptoStream(ms, sa.CreateDecryptor(), CryptoStreamMode.Write);

                    cs.Write(content, 0, content.Length);
                    cs.FlushFinalBlock();
                    var password = Encoding.UTF8.GetString(ms.ToArray());

                    data.Item1 = User.User;
                    data.Item2 = password;

                    list.Add(data);
                }

                return list;

            }
            catch (Exception ex)

            {
                return null;
            }


        }

        public void InitConfig()
        {
            if (Config.GetSection("UserCredential") == null)
            {
                Config.Sections.Add("UserCredential", new UserSection());
            }

            Config.Save(ConfigurationSaveMode.Modified);
        }
    }
}
