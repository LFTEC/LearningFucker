using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using LearningFucker.Models;
namespace LearningFucker
{
    public class DataContext
    {

        public Question GetRow(int tmid)
        {
            try
            {
                Question question = new Question();
                using (MySqlConnection connection = new MySqlConnection(""))
                {
                    MySqlCommand command = new MySqlCommand("select * from tm where tmid = @tmid", connection);
                    MySqlParameter parameter = new MySqlParameter("tmid", tmid);
                    command.Parameters.Add(parameter);
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandTimeout = 10;
                    connection.Open();

                    var reader = command.ExecuteReader();
                    if(reader.HasRows)
                    {
                        reader.Read();

                        
                        question.TmID = Convert.ToInt32(reader["tmid"]);
                        question.TkID = Convert.ToInt32(reader["tkid"]);
                        question.TmSourceType = Convert.ToInt32(reader["type"]);
                        question.TmTx = Convert.ToInt32(reader["tmtx"]);
                        question.TmBaseTx = Convert.ToString(reader["txtext"]);
                        question.TmTxStr = Convert.ToString(reader["txstr"]);
                        question.Title = Convert.ToString(reader["title"]);
                        question.TmKey = Convert.ToString(reader["tmkey"]);
                        question.Options = Convert.ToString(reader["options"]);
                        question.Answers = Convert.ToString(reader["answers"]);
                        question.Difficulty = Convert.ToString(reader["difficulty"]);
                        question.Remark = Convert.ToString(reader["remark"]);
                        question.Score = Convert.ToDecimal(reader["score"]);
                        reader.Close();
                        
                    }

                    connection.Close();
                    return question;

                }
            }
            catch(Exception ex)
            {
                return null;
            }
            
        }

        public bool InsertRow(Question question)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(""))
                {
                    string sql;
                    sql = @"insert tm values(@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13)";
                    MySqlCommand command = new MySqlCommand("select * from tm where tmid = @tmid", connection);
                    MySqlParameter parameter = new MySqlParameter("@1", question.TmID);
                    command.Parameters.Add(parameter);
                    parameter = new MySqlParameter("@2", question.TkID);
                    command.Parameters.Add(parameter);
                    parameter = new MySqlParameter("@3", question.TmSourceType);
                    command.Parameters.Add(parameter);
                    parameter = new MySqlParameter("@4", question.TmTx);
                    command.Parameters.Add(parameter);
                    parameter = new MySqlParameter("@5", question.TmBaseTx);
                    command.Parameters.Add(parameter);
                    parameter = new MySqlParameter("@6", question.TmTxStr);
                    command.Parameters.Add(parameter);
                    parameter = new MySqlParameter("@7", question.Title);
                    command.Parameters.Add(parameter);
                    parameter = new MySqlParameter("@8", question.TmKey);
                    command.Parameters.Add(parameter);
                    parameter = new MySqlParameter("@9", question.Options);
                    command.Parameters.Add(parameter);
                    parameter = new MySqlParameter("@10", question.Answers);
                    command.Parameters.Add(parameter);
                    parameter = new MySqlParameter("@11", question.Difficulty);
                    command.Parameters.Add(parameter);
                    parameter = new MySqlParameter("@12", question.Remark);
                    command.Parameters.Add(parameter);
                    parameter = new MySqlParameter("@13", question.Score);
                    command.Parameters.Add(parameter);
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandTimeout = 10;
                    connection.Open();

                    var result = command.ExecuteNonQuery();
                    connection.Close();
                    return result == 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
