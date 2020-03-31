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
using System.Configuration;
using System.Security.Cryptography;
using System.Diagnostics;
using System.IO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;

namespace LearningFucker
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        public Form1()        {
            
            InitializeComponent();
            layoutLogin.Dock = DockStyle.Fill;
            layoutTask.Dock = DockStyle.Fill;
            Worker = new Worker();
            Worker.TaskRefresed = new Action<Worker>(s => gridControl1.RefreshDataSource());
            Config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            ReadPassword();
        }

        private const string KEY = "jcflRWUJqAs=";
        private const string IV = "hBoIpG2rhqE=";


        public Worker Worker { get; set; }
        Configuration Config { get; set; }

        private void BarBtnLogin_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            layoutLogin.Visible = true;
            layoutTask.Visible = false;
            Worker.StopWork();
            
        }

        private async void SimpleButton11_Click(object sender, EventArgs e)
        {
            dxError.ClearErrors();
            var userId = textEdit11.Text;
            var password = textEdit2.Text;
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
            {

            }
            else
            {
                userId = userId.Trim();
                password = password.Trim();
                var login = await Worker.Login(userId, password);
                if (login)
                {
                    if (checkEdit1.Checked)
                        SavePassword(userId, password);

                    layoutLogin.Visible = false;
                    layoutTask.Visible = true;

                    await Worker.Init();
                    //var taskList = new List<int>();
                    //taskList.Add(14);
                    //Worker.StartWork(taskList, false);
                }
                else
                {
                    dxError.SetError(textEdit11, "用户名密码不正确");
                    barStatusText.Caption = "用户名密码不正确, 登陆失败!";
                }
            }
        }

        private void SavePassword(string userId, string password)
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

            if(Config.AppSettings.Settings["UserId"] == null)
            {
                Config.AppSettings.Settings.Add("UserId", userId);
            }
            else
                Config.AppSettings.Settings["UserId"].Value = userId;

            if (Config.AppSettings.Settings["Password"] == null)
            {
                Config.AppSettings.Settings.Add("Password", cryPassword);
            }
            else
                Config.AppSettings.Settings["Password"].Value = cryPassword;

            Config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void ReadPassword()
        {
            if (Config.AppSettings.Settings["UserId"] == null)
            {
                textEdit11.Text = "";
            }
            else
                textEdit11.Text = Config.AppSettings.Settings["UserId"].Value;

            if (Config.AppSettings.Settings["Password"] == null)
            {
                textEdit2.Text = "";
            }
            else
            {
                SymmetricAlgorithm sa = DES.Create();
                sa.Key = Convert.FromBase64String(KEY);
                sa.IV = Convert.FromBase64String(IV);
                var cryPassword = Config.AppSettings.Settings["Password"].Value;

                byte[] content = Convert.FromBase64String(cryPassword);
                var ms = new MemoryStream();
                var cs = new CryptoStream(ms, sa.CreateDecryptor(), CryptoStreamMode.Write);
                
                cs.Write(content, 0, content.Length);
                cs.FlushFinalBlock();
                textEdit2.Text = Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.bindingSource1.DataSource = Worker.TaskList;

            repositoryItemImageComboBox1.Items.Add("", LearningFucker.Handler.TaskStatus.Initial, 2);
            repositoryItemImageComboBox1.Items.Add("", LearningFucker.Handler.TaskStatus.Completed, 3);
            repositoryItemImageComboBox1.Items.Add("", LearningFucker.Handler.TaskStatus.Stopped, 2);
            repositoryItemImageComboBox1.Items.Add("", LearningFucker.Handler.TaskStatus.Stopping, 2);
            repositoryItemImageComboBox1.Items.Add("", LearningFucker.Handler.TaskStatus.Working, 1);
        }



        private void GridView1_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName != "IsSelect") return;
            var grid = sender as GridView;
            var row = grid.GetRow(e.RowHandle) as LearningFucker.Models.Task;


            if (row.LimitIntegral <= row.Integral)
            {
                e.RepositoryItem.ReadOnly = true;

            }
            else
            {
                e.RepositoryItem.ReadOnly = false;
            }
        }

        private void BarButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Worker.Refresh();
        }

        private void BarButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            var selectedTask = Worker.TaskList.Where(s => s.IsSelect);
            
            if(selectedTask.Count() == 0)
            {
                XtraMessageBox.Show("请选择要刷分的任务!", "请注意!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                List<int> tasks = new List<int>();
                foreach (var item in selectedTask)
                {
                    tasks.Add(item.TaskType);
                }

                Worker.StartWork(tasks, false);
            }
        }
    }

    

    
}
