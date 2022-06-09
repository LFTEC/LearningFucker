using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using LearningFucker.Models;
namespace LearningFucker
{
    public class DataContext
    {

        private static Object write_lock = new object();

        public async Task<Question> GetRow(int tmid)
        {

            Question question = new Question();
            using(SqlConnection connection = new SqlConnection("Data Source=localhost;database=learning;Uid=user;Pwd=LearningFucker2020;"))
            {
                SqlCommand command = new SqlCommand("select * from tm where tmid = @tmid;", connection);
                SqlParameter parameter = new SqlParameter("tmid", tmid);
                command.Parameters.Add(parameter);
                command.CommandType = System.Data.CommandType.Text;
                command.CommandTimeout = 10;
                await connection.OpenAsync();

                var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {

                    await reader.ReadAsync();

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
                    connection.Close();
                    return question;
                }
                else
                {
                    connection.Close();
                    return null;
                }

            }
            
        }

        public async Task<bool> InsertRow(Question question)
        {

            using (SqlConnection connection = new SqlConnection("Data Source=localhost;database=learning;Uid=user;Pwd=LearningFucker2020;"))
            {
                string sql;
                sql = @"insert tm values(@1,@2,@3,@4,@5,@6,@7,@8,@9,@10,@11,@12,@13);";
                SqlCommand command = new SqlCommand(sql, connection);
                SqlParameter parameter = new SqlParameter("@1", question.TmID);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@2", question.TkID);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@3", question.TmSourceType);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@4", question.TmTx);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@5", question.TmBaseTx);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@6", question.TmTxStr);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@7", question.Title);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@8", question.TmKey);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@9", question.Options);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@10", question.Answers);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@11", question.Difficulty);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@12", question.Remark);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@13", question.Score);
                command.Parameters.Add(parameter);
                command.CommandType = System.Data.CommandType.Text;
                command.CommandTimeout = 10;
                await connection.OpenAsync();
                var result = await command.ExecuteNonQueryAsync();
                connection.Close();
                return result == 0 ? true : false;
            }

        }

        public async Task<bool> UpdateRow(Question question)
        {

            using (SqlConnection connection = new SqlConnection("Data Source=localhost;database=learning;Uid=user;Pwd=LearningFucker2020;"))
            {
                string sql;
                sql = @"update tm set tkid=@2,type=@3,tmtx=@4,txtext=@5,txstr=@6,title=@7,tmkey=@8,options=@9,answers=@10,difficulty=@11,
                        remark=@12,score=@13 where tmid=@1;";
                SqlCommand command = new SqlCommand(sql, connection);
                SqlParameter parameter = new SqlParameter("@1", question.TmID);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@2", question.TkID);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@3", question.TmSourceType);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@4", question.TmTx);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@5", question.TmBaseTx);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@6", question.TmTxStr);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@7", question.Title);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@8", question.TmKey);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@9", question.Options);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@10", question.Answers);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@11", question.Difficulty);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@12", question.Remark);
                command.Parameters.Add(parameter);
                parameter = new SqlParameter("@13", question.Score);
                command.Parameters.Add(parameter);
                command.CommandType = System.Data.CommandType.Text;
                command.CommandTimeout = 10;
                await connection.OpenAsync();

                var result = await command.ExecuteNonQueryAsync();
                connection.Close();
                return result == 0 ? true : false;
            }

        }

        public async Task<bool> WriteData(Question? item)
        {
            if (item == null) return false;
            var row = await GetRow(item.TmID);
            bool result = false;
            if (row == null)
            {
                result = await InsertRow(item);
            }
            else if (row.Answers != item.Answers)
            {
                result = await UpdateRow(item);
            }
            else result = true;
            return result;
        }
    }
}
