using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.UserSkins;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace LearningFucker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            BonusSkins.Register();
            Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://server.jcdev.cc:9200"))
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6
                    })
                    .Enrich.WithProperty("guid", System.Guid.NewGuid())
                    .CreateLogger();
            try
            {
                Application.Run(new Form1());
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
