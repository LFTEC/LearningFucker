using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Web;
using LearningFucker.Models;

namespace LearningFucker
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        public Form1()        {
            
            InitializeComponent();
            Worker = new Worker();
        }


        public Worker Worker { get; set; }

        private async void BarBtnLogin_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var userId = textEdit11.Text;
            var password = textEdit2.Text;
            if(string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
            {

            }
            else
            {
                var login = await Worker.Login(userId, password);
                if(login)
                {
                    await Worker.Init();
                    var taskList = new List<int>();
                    taskList.Add(14);
                    Worker.StartWork(taskList, false);
                }
            }

        }

        
    }

    

    
}
