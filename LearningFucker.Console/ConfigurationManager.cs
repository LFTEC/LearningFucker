using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.IO;

namespace LearningFucker.CLI
{
    public class ConfigurationManager
    {
        public static Configuration Configuration = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private const string KEY = "jcflRWUJqAs=";
        private const string IV = "hBoIpG2rhqE=";

        public static UserInfo GetDefaultUser()
        {
            string userid = "", password = "";
            if(!Configuration.AppSettings.Settings.AllKeys.Contains("Query"))
            {
                Configuration.AppSettings.Settings.Add("Query", "");
            }

            string query = Configuration.AppSettings.Settings["Query"].Value;
            if ( string.IsNullOrEmpty(query) )
            {
                ResetDefaultUser();
            }
            else
            {
                var strList = query.Split(',');
                if(strList.Length < 2)
                {
                    ResetDefaultUser();
                }
                else
                {
                    userid = strList[0];
                    password = strList[1];
                    userid = GetDecryptString(userid);
                    password = GetDecryptString(password);
                }
            }

            UserInfo userInfo = new UserInfo();
            userInfo.UserId = userid;
            userInfo.Password = password;
            return userInfo;
        }

        public static void ResetDefaultUser()
        {
            string userid = Program.ReadInfo("please input default user id: ");
            string password = Program.ReadInfo("please input default user password: ");
            SetDefaultUser(userid, password);
        }

        private static void SetDefaultUser(string userid, string password)
        {
            userid = GetEncryptString(userid);
            password = GetEncryptString(password);
            var text = userid + "," + password;
            if (Configuration.AppSettings.Settings.AllKeys.Contains("Query"))
            {
                Configuration.AppSettings.Settings["Query"].Value = text;
            }
            else
                Configuration.AppSettings.Settings.Add("Query", text);
            Configuration.Save(ConfigurationSaveMode.Modified);
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
        }

        private static string GetDecryptString(string encryptString)
        {
            SymmetricAlgorithm sa = DES.Create();
            sa.Key = Convert.FromBase64String(KEY);
            sa.IV = Convert.FromBase64String(IV);
            byte[] content = Convert.FromBase64String(encryptString);

            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, sa.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(content, 0, content.Length);
            cs.FlushFinalBlock();
            var cryString = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return cryString;
        }

        private static string GetEncryptString(string text)
        {
            SymmetricAlgorithm sa = DES.Create();
            sa.Key = Convert.FromBase64String(KEY);
            sa.IV = Convert.FromBase64String(IV);
            byte[] content = Encoding.UTF8.GetBytes(text);

            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, sa.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(content, 0, content.Length);
            cs.FlushFinalBlock();
            var cryString = Convert.ToBase64String(ms.ToArray());
            return cryString;
        }
    }
}
