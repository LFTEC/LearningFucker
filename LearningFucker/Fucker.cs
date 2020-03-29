using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using LearningFucker.Models;

namespace LearningFucker
{
    public class Fucker
    {
        public Fucker()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://learning.whchem.com:4443");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla /5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1");
            
            
        }

        

        private HttpClient httpClient;
        private string user_token;
        public const int POLLING_TIME = 30000;      

        

        public string UserToken
        {
            get => user_token;
            set
            {
                user_token = value;
                if (httpClient.DefaultRequestHeaders.Contains("authorization"))
                    httpClient.DefaultRequestHeaders.Remove("authorization");
                httpClient.DefaultRequestHeaders.Add("authorization", user_token);
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<User> Login(string userId, string password)
        {
            try
            {
                Dictionary<string, string> token = new Dictionary<string, string>();
                token.Add("username", userId);
                token.Add("password", password);

                HttpContent httpContent;
                httpContent = new FormUrlEncodedContent(token);
                httpContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
                User user = await Post<User>("Api/User/Login", httpContent);
                if (user != null && !string.IsNullOrEmpty(user.UserName))
                {
                    UserToken = user.Token;
                    return user;
                }
                else
                {
                    UserToken = "";
                    return null;
                }
            }
            catch (Exception ex)
            {
                UserToken = "";
                return null;
            }
        }

        /// <summary>
        /// 获取APP信息
        /// </summary>
        /// <returns></returns>
        public async Task<AppInfo> GetAppInfo()
        {
            try
            {
                return await Get<AppInfo>("Api/Common/GetAppConfigInfo", null);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取用户统计信息
        /// </summary>
        /// <returns></returns>
        public async Task<UserStatistics> GetMyTaskInfo()
        {
            try
            {
                return await Get<UserStatistics>("Api/Common/Task/GetMyTaskInfo", null);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取每日任务列表
        /// </summary>
        /// <param name="taskGroup"></param>
        /// <returns></returns>
        public async Task<TaskList> GetTaskList(string taskGroup)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("taskGroup", taskGroup);
                return await Post<TaskList>("Api/Common/Task/GetTaskList", GetContent(valuePairs));
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 刷新用户信息
        /// </summary>
        /// <returns></returns>
        public async Task<User> GetUserInfo()
        {
            try
            {
                var user = await Get<User>("Api/User/GetMyInfo", null);
                if (user != null && !string.IsNullOrEmpty(user.UserName))
                {
                    UserToken = user.Token;
                    return user;
                }
                else
                {
                    UserToken = "";
                    return null;
                }
            }
            catch (Exception ex)
            {
                UserToken = "";
                return null;
            }
        }

        /// <summary>
        /// 获取必修课程列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public async Task<CourseList> GetCourseList(int pageIndex, int pageCount)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("pageIndex", pageIndex.ToString());
                valuePairs.Add("pageCount", pageCount.ToString());
                var list = await Post<CourseList>("Api/TaskStudy/GetList", GetContent(valuePairs));
                if(list != null && list.List != null)
                {
                    foreach (var item in list.List)
                    {
                        item.ProjType = "3";
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async System.Threading.Tasks.Task GetCourseDetail(Course course)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("ProjType", course.ProjType);
                valuePairs.Add("ProjID", course.ProjID);
                valuePairs.Add("Proj2ID", course.Proj2ID);
                var detail = await Get<CourseDetail>("Api/Courseware/Study/GetCoursewares", valuePairs);
                course.Detail = detail;
            }
            catch (Exception ex)
            {
                
            }
        }

        public async System.Threading.Tasks.Task GetCourseDetail(ElectiveCourse course)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("ProjType", course.ProjType);
                valuePairs.Add("ProjID", course.ID);
                valuePairs.Add("Proj2ID", course.Proj2ID);
                var detail = await Get<CourseDetail>("Api/Courseware/Study/GetCoursewares", valuePairs);
                course.Detail = detail;
            }
            catch (Exception ex)
            {

            }
        }

        public async System.Threading.Tasks.Task GetCourseAppendix(Course course)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("ProjType", course.ProjType);
                valuePairs.Add("ProjID", course.ProjID);
                valuePairs.Add("Proj2ID", course.Proj2ID);
                var appendix = await Get<CourseAppendix>("Api/Courseware/PlayPage/PageInit", valuePairs);
                course.Appendix = appendix;
            }
            catch (Exception ex)
            {

            }
        }

        public async System.Threading.Tasks.Task GetCourseAppendix(ElectiveCourse course)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("ProjType", course.ProjType);
                valuePairs.Add("ProjID", course.ID);
                valuePairs.Add("Proj2ID", course.Proj2ID);
                var appendix = await Get<CourseAppendix>("Api/Courseware/PlayPage/PageInit", valuePairs);
                course.Appendix = appendix;
            }
            catch (Exception ex)
            {

            }
        }

        public async Task<Study> StartStudy(Course course, WareDetail ware)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("ProjType", course.ProjType);
                valuePairs.Add("ProjID", course.ProjID);
                valuePairs.Add("Proj2ID", course.Proj2ID);
                valuePairs.Add("WareID", ware.WareId);
                var logId = await Post<string>("Api/Courseware/Study/StartStudy", GetContent(valuePairs));
                Study study = new Study();
                study.LogId = logId;
                study.ProjType = course.ProjType;
                study.ProjID = course.ProjID;
                study.Proj2ID = course.Proj2ID;
                study.WareID = ware.WareId;
                study.Course = course.Detail;

                study.Appendix = course.Appendix;
                study.Ware = ware;
                return study;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Study> StartStudy(ElectiveCourse course, WareDetail ware)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("ProjType", course.ProjType);
                valuePairs.Add("ProjID", course.ID);
                valuePairs.Add("Proj2ID", course.Proj2ID);
                valuePairs.Add("WareID", ware.WareId);
                var logId = await Post<string>("Api/Courseware/Study/StartStudy", GetContent(valuePairs));
                Study study = new Study();
                study.LogId = logId;
                study.ProjType = course.ProjType;
                study.ProjID = course.ID;
                study.Proj2ID = course.Proj2ID;
                study.WareID = ware.WareId;
                study.Course = course.Detail;
                study.Appendix = course.Appendix;
                study.Ware = ware;
                return study;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> GetStudyInfo(Study study)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("ProjType", study.ProjType);
                valuePairs.Add("ProjID", study.ProjID);
                valuePairs.Add("Proj2ID", study.Proj2ID);
                valuePairs.Add("LogID", study.LogId);

                var tmpStudy = await Post<Study>("Api/Courseware/PlayPage/GetStudyInfo", GetContent(valuePairs));
                study.SumStudyTime = tmpStudy.SumStudyTime;
                study.TodayStudyTime = tmpStudy.TodayStudyTime;
                study.ExamIntegral = tmpStudy.ExamIntegral;
                study.StudyIntegral = tmpStudy.StudyIntegral;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SaveStudyLog(Study study)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("ProjType", study.ProjType);
                valuePairs.Add("ProjID", study.ProjID);
                valuePairs.Add("Proj2ID", study.Proj2ID);
                valuePairs.Add("WareID", study.WareID);
                valuePairs.Add("LogID", study.LogId);
                valuePairs.Add("Timed", Convert.ToString(POLLING_TIME / 1000));
                valuePairs.Add("studyStatus", "1");

                var tmpStudy = await Post<Study>("Api/Courseware/Study/SaveStudyLog", GetContent(valuePairs));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<ExamList> GetExamList(Course course)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("taskId", course.ProjID);
                var examList = await Get<ExamList>("Api/TaskStudy/GetExamList", valuePairs);
                examList.ProjID = course.ProjID;
                if(examList.List != null)
                {
                    examList.List.ForEach(s => s.ProjID = course.ProjID);
                }
                return examList;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public async System.Threading.Tasks.Task GetExamDetail(Exam exam)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("examId", exam.ExamID);
                var examDetail = await Get<ExamDetail>("Api/Exam/GetExamDetail", valuePairs);
                exam.ExamDetail = examDetail;
            }
            catch (Exception ex)
            {
                
            }
        }

        public async Task<bool> StartExam(Exam exam)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("examId", exam.ExamID);
                var paper = await Post<Paper>("Api/Exam/StartExam", GetContent(valuePairs));
                if (exam.Papers == null)
                {
                    exam.Papers = new List<Paper>();
                }
                exam.Papers.Add(paper);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> StartExercise(ElectiveCourse course)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("mode", "1");
                valuePairs.Add("projId", course.ID);
                valuePairs.Add("projType", course.ProjType);
                valuePairs.Add("wareId", "");
                valuePairs.Add("questionCount", "10");

                var paper = await Get<Exercise>("Api/Courseware/Exercise/GetExerciseQuestions", valuePairs);
                if (course.Exercises == null)
                {
                    course.Exercises = new List<Exercise>();
                }
                course.Exercises.Add(paper);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> HandIn(Paper paper, Exam exam, List<Answer> answers)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("examId", exam.ExamID);
                valuePairs.Add("cacheId", paper.PaperId);
                valuePairs.Add("referenceId", exam.ProjID);
                valuePairs.Add("answers", JsonConvert.SerializeObject(answers));
                var result = await Post<Result>("Api/Exam/Submit", GetContent(valuePairs));
                paper.Result = result;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> HandIn(ElectiveCourse course, Exercise exercise, List<ExerciseAnswer> answers, int exerciseTime)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("mode", "1");
                valuePairs.Add("projId", course.ID);
                valuePairs.Add("projType", course.ProjType);
                valuePairs.Add("wareId", "");
                valuePairs.Add("second", exerciseTime.ToString());
                valuePairs.Add("wareId", JsonConvert.SerializeObject(answers));
                var result = await Get<string>("Api/Courseware/Exercise/SubmitExerciseAnswer", valuePairs);
                exercise.Result.ResultId = result;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> GetResult(Result result)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("resultId", result.ResultId);
                var tmpResult = await Get<Result>("Api/Exam/GetJudgeState", valuePairs);
                result.State = tmpResult.State;
                result.Score = tmpResult.Score;
                result.ElapsedMinutes = tmpResult.ElapsedMinutes;
                result.ErrorQuestionCount = tmpResult.ErrorQuestionCount;
                result.Integral = tmpResult.Integral;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> GetResult(ExerciseResult result)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("resultId", result.ResultId);
                var tmpResult = await Get<dynamic>("Api/Courseware/Exercise/GetExerciseResult", valuePairs);

                result.Status = tmpResult.exercise.Status;
                result.Integral = tmpResult.Integral;

                ExerciseResult tmpResult2 = JsonConvert.DeserializeObject<ExerciseResult>(JsonConvert.SerializeObject(tmpResult.exercise.Result));
                result.AddTime = tmpResult2.AddTime;
                result.EndTime = tmpResult2.EndTime;
                result.RightCount = tmpResult2.RightCount;
                result.WrongCount = tmpResult2.WrongCount;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> ReviewPaper(Result result, Exam exam)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("examId", exam.ExamID);
                valuePairs.Add("resultId", result.ResultId);
                var tmpResult = await Get<Result>("Api/Exam/GetExamResult", valuePairs);
                result.Questions = tmpResult.Questions;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> ReviewExercise(ExerciseResult result)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("resultId", result.ResultId);
                var tmpResult = await Get<ExerciseResult>("Api/Courseware/Exercise/GetResultQuestions", valuePairs);
                result.Questions = tmpResult.Questions;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<PropertyList> GetPropertyList()
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("resourceCategory", "32");
                var propertyList = await Post<PropertyList>("Api/Common/GetResourceProperties", GetContent(valuePairs));
                return propertyList;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<ElectiveCourseList> GetElectiveCourseList(Context context)
        {
            try
            {
                Dictionary<string, string> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("name", "");
                valuePairs.Add("propertyId", context.PropertyId);
                valuePairs.Add("contextId", context.Id);
                valuePairs.Add("pageIdx", "0");
                valuePairs.Add("pageSize", "1000");
                var electiveCourseList = await Get<ElectiveCourseList>("Api/CourseStudy/GetCourses", valuePairs);
                if (electiveCourseList != null && electiveCourseList.List != null)
                {
                    foreach (var item in electiveCourseList.List)
                    {
                        item.ProjType = "0";
                    }
                }
                return electiveCourseList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        private HttpContent GetContent(IEnumerable<KeyValuePair<string, string>> body)
        {
            if (string.IsNullOrEmpty(user_token))
                throw new Exception("未登录");
            HttpContent httpContent;
            httpContent = new FormUrlEncodedContent(body);
            httpContent.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            return httpContent;
        }

        private async Task<T> Get<T>(string requestUrl, IEnumerable<KeyValuePair<string, string>> pairs) where T : class
        {
            Random random = new Random();
            var ran = random.NextDouble().ToString();

            requestUrl = string.Format("{0}?random={1}", requestUrl, ran);
            if (pairs != null && pairs.Count() > 0)
            {
                var querystring = pairs.Aggregate("", (current, item) => string.Format("{0}{1}={2}&", current, item.Key, HttpUtility.UrlEncode(item.Value, Encoding.UTF8)));
                querystring = querystring.Substring(0, querystring.LastIndexOf("&"));
                requestUrl = requestUrl + "&" + querystring;
            }

            var response = await httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseContentRead);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("网络访问异常");
            }

            var result = await response.Content.ReadAsStringAsync();
            dynamic obj = JsonConvert.DeserializeObject(result);
            if (obj.state != "success")
                throw new Exception("接口返回数据异常!");

            T data = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj.data));
            return data;
        }

        private async Task<T> Post<T>(string requestUrl, HttpContent content) where T : class
        {
            var response = await httpClient.PostAsync(requestUrl, content);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("网络访问异常");
            }

            var result = await response.Content.ReadAsStringAsync();
            dynamic obj = JsonConvert.DeserializeObject(result);
            if (obj.state != "success")
                throw new Exception("接口返回数据异常!");

            T data = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj.data));
            return data;
        }

    }
}
