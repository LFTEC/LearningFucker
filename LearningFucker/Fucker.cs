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
using System.Collections;

namespace LearningFucker
{
    public class Fucker
    {
        public Fucker(Worker worker)
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://learning.whchem.com:4443");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla /5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1");

            this.worker = worker;
        }

        

        private HttpClient httpClient;
        private string user_token;
        public const int POLLING_TIME = 30000;
        private Worker worker;

        public Worker Worker { get=>worker; }
        

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
                return null;
            }
        }

        /// <summary>
        /// 获取APP信息
        /// </summary>
        /// <returns></returns>
        public async Task<AppInfo> GetAppInfo()
        {
            return await Get<AppInfo>("Api/Common/GetAppConfigInfo", null);
        }

        /// <summary>
        /// 获取用户统计信息
        /// </summary>
        /// <returns></returns>
        public async Task<UserStatistics> GetMyTaskInfo()
        {
            var userStat = await Get<UserStatistics>("Api/Common/Task/GetMyTaskInfo", null);
            var tmpUserStat = await Post<UserStatistics>("Api/Integral/GetSummaries", null);
            userStat.WeekIntegral = tmpUserStat.WeekIntegral;
            return userStat;
           
        }

        /// <summary>
        /// 获取每日任务列表
        /// </summary>
        /// <param name="taskGroup"></param>
        /// <returns></returns>
        public async Task<TaskList> GetTaskList(string taskGroup)
        {
            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("taskGroup", taskGroup);
            return await Post<TaskList>("Api/Common/Task/GetTaskList", GetContent(valuePairs));
        }

        /// <summary>
        /// 刷新用户信息
        /// </summary>
        /// <returns></returns>
        public async Task<User> GetUserInfo()
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

        /// <summary>
        /// 获取必修课程列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageCount"></param>
        /// <returns></returns>
        public async Task<CourseList> GetCourseList(int pageIndex, int pageCount)
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

        public async System.Threading.Tasks.Task GetCourseDetail(Course course)
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("ProjType", course.ProjType);
            valuePairs.Add("ProjID", course.ProjID);
            valuePairs.Add("Proj2ID", course.Proj2ID);
            var detail = await Get<CourseDetail>("Api/Courseware/Study/GetCoursewares", valuePairs);
            course.Detail = detail;
        }

        public async System.Threading.Tasks.Task GetCourseDetail(ElectiveCourse course)
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("ProjType", course.ProjType);
            valuePairs.Add("ProjID", course.ID);
            valuePairs.Add("Proj2ID", course.Proj2ID);
            var detail = await Get<CourseDetail>("Api/Courseware/Study/GetCoursewares", valuePairs);
            course.Detail = detail;
        }

        public async System.Threading.Tasks.Task GetCourseAppendix(Course course)
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("ProjType", course.ProjType);
            valuePairs.Add("ProjID", course.ProjID);
            valuePairs.Add("Proj2ID", course.Proj2ID);
            var appendix = await Get<CourseAppendix>("Api/Courseware/PlayPage/PageInit", valuePairs);
            course.Appendix = appendix;
        }

        public async System.Threading.Tasks.Task GetCourseAppendix(ElectiveCourse course)
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("ProjType", course.ProjType);
            valuePairs.Add("ProjID", course.ID);
            valuePairs.Add("Proj2ID", course.Proj2ID);
            var appendix = await Get<CourseAppendix>("Api/Courseware/PlayPage/PageInit", valuePairs);
            course.Appendix = appendix;
        }

        public async Task<Study> StartStudy(Course course, WareDetail ware)
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

        public async Task<Study> StartStudy(ElectiveCourse course, WareDetail ware)
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

        public async Task<bool> GetStudyInfo(Study study)
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

        public async Task<bool> SaveStudyLog(Study study)
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

        public async Task<ExamList> GetExamList(Course course)
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("taskId", course.ProjID);
            var examList = await Get<ExamList>("Api/TaskStudy/GetExamList", valuePairs);
            examList.ProjID = course.ProjID;
            if (examList.List != null)
            {
                examList.List.ForEach(s => s.ProjID = course.ProjID);
            }
            return examList;
        }

        public async System.Threading.Tasks.Task GetExamDetail(Exam exam)
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("examId", exam.ExamID);
            var examDetail = await Get<ExamDetail>("Api/Exam/GetExamDetail", valuePairs);
            exam.ExamDetail = examDetail;
        }

        public async Task<bool> StartExam(Exam exam)
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

        public async Task<bool> StartExercise(ElectiveCourse course)
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

        public async Task<bool> HandIn(Paper paper, Exam exam, List<Answer> answers)
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

        public async Task<bool> HandIn(ElectiveCourse course, Exercise exercise, List<ExerciseAnswer> answers, int exerciseTime)
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("mode", "1");
            valuePairs.Add("projId", course.ID);
            valuePairs.Add("projType", course.ProjType);
            valuePairs.Add("wareId", "");
            valuePairs.Add("second", exerciseTime.ToString());
            valuePairs.Add("answer", JsonConvert.SerializeObject(answers));
            var result = await Get<string>("Api/Courseware/Exercise/SubmitExerciseAnswer", valuePairs);
            exercise.Result = new ExerciseResult();
            exercise.Result.ResultId = result;
            return true;
        }

        public async Task<bool> GetResult(Result result)
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

        public async Task<bool> GetResult(ExerciseResult result)
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

        public async Task<bool> ReviewPaper(Result result, Exam exam)
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("examId", exam.ExamID);
            valuePairs.Add("resultId", result.ResultId);
            var tmpResult = await Get<Result>("Api/Exam/GetExamResult", valuePairs);
            result.Questions = tmpResult.Questions;
            return true;
        }

        public async Task<bool> ReviewExercise(ExerciseResult result)
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("resultId", result.ResultId);
            var tmpResult = await Get<ExerciseResult>("Api/Courseware/Exercise/GetResultQuestions", valuePairs);
            result.Questions = tmpResult.Questions;
            return true;
        }

        public async Task<PropertyList> GetPropertyList()
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("resourceCategory", "32");
            var propertyList = await Post<PropertyList>("Api/Common/GetResourceProperties", GetContent(valuePairs));
            return propertyList;
        }

        public async Task<ElectiveCourseList> GetElectiveCourseList(Context context)
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

        public async Task<BreakthroughList> GetBreakthroughList()
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("pageSize", "2000");
            valuePairs.Add("pageIndex", "0");
            var breakthroughList = await Post<BreakthroughList>("Api/PointAnswer/GetPointAnswerDetail", GetContent(valuePairs));
            return breakthroughList;
        }

        public async Task<bool> StartBreakthrough(Breakthrough breakthrough)
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("level", breakthrough.PointLevelId);
            valuePairs.Add("isExercise", "false");
            var tmpBreakthrough = await Post<Breakthrough>("Api/PointAnswer/GetPointAnswerQuestion", GetContent(valuePairs));

            breakthrough.Questions = tmpBreakthrough.Questions;
            return true;
        }

        public async Task<bool> HandIn(Breakthrough breakthrough, List<ExerciseAnswer> answer, int answerTime)
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("level", breakthrough.PointLevelId);
            valuePairs.Add("answer", JsonConvert.SerializeObject(answer));
            valuePairs.Add("second", answerTime.ToString());
            var result = await Post<BreakthroughResult>("Api/PointAnswer/SubmitPointAnswer", GetContent(valuePairs));

            breakthrough.Result = result;
            return true;
        }


        public async Task<bool> GetResult(BreakthroughResult result)
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("resultId", result.ResultId);
            var tmpResult = await Post<dynamic>("Api/PointAnswer/GetPointAnswerResult", GetContent(valuePairs));

            result.Status = tmpResult.pointAnswerResult.Status;
            result.Integral = tmpResult.pointAnswerIntegral;

            BreakthroughResult tmpResult2 = JsonConvert.DeserializeObject<BreakthroughResult>(JsonConvert.SerializeObject(tmpResult.pointAnswerResult.Result));

            result.BTime = tmpResult2.BTime;
            result.ETime = tmpResult2.ETime;
            result.UseSec = tmpResult2.UseSec;
            result.Score = tmpResult2.Score;
            result.IsPass = tmpResult2.IsPass;
            result.TotalNum = tmpResult2.TotalNum;
            result.RightNum = tmpResult2.RightNum;
            result.Answer = tmpResult2.Answer;

            return true;
        }


        public async Task<bool> ReviewBreakthrough(BreakthroughResult result)
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("resultId", result.ResultId);
            var tmpResult = await Get<BreakthroughResult>("Api/PointAnswer/GetResultQuestions", valuePairs);

            result.Questions = tmpResult.Questions;

            return true;
        }



        /// <summary>
        /// 获取PK信息
        /// </summary>
        /// <returns></returns>
        public async Task<CombatList> GetPKInfo()
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("pageIndex", "0");
            valuePairs.Add("pageSize", "10");
            valuePairs.Add("isShow", "0");
            var combatList = await Post<CombatList>("Api/UserPK/GetInfo", GetContent(valuePairs));
            return combatList;
        }


        /// <summary>
        /// 获取竞技场
        /// </summary>
        /// <returns></returns>
        public async Task<Arena> GetArena()
        {

            var arena = await Post<Arena>("Api/UserPK/IsJoinPK", null);
            await Post<string>("Api/UserPK/IsTmCount", null);
            return arena;
        }

        /// <summary>
        /// 加入竞技场
        /// </summary>
        /// <param name="arena"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> JoinArena(Arena arena)
        {

            var tmpArena = await Get<Arena>("Api/UserPK/JoinPK", null);
            if (string.IsNullOrEmpty(tmpArena.GroupId))
            {
                arena.GroupId = "";
                return false;
            }
            else
            {
                arena.GroupId = tmpArena.GroupId;
                return true;
            }
        }

        /// <summary>
        /// 获取对战双方信息
        /// </summary>
        /// <param name="arena"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> GetArenaBothSide(Arena arena)
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("groupID", arena.GroupId);

            var list = await Post<List<List<Gladiator>>>("Api/UserPK/GetGroupUser", GetContent(valuePairs));
            if (list != null)
            {
                arena.BothSides = list[0];
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 准备战斗
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> ReadyToFight()
        {

            await Post<string>("Api/UserPK/EndJoin", null);
            return true;
        }

        /// <summary>
        /// 开始战斗
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<Round> Fight(Arena arena)
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("groupID", arena.GroupId);
            valuePairs.Add("mode", "0");

            var round = await Get<Round>("Api/UserPK/GetCurrentStatus", valuePairs);
            return round;
        }

        /// <summary>
        /// 提交PK答案
        /// </summary>
        /// <param name="arena"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> SubmitQuestion(Arena arena, Round round, string answer)
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("groupID", arena.GroupId);
            valuePairs.Add("mode", "0");
            valuePairs.Add("index", round.CurrentIndex.ToString());
            valuePairs.Add("answer", answer);

            await Post<string>("Api/UserPK/SubmitQuestion", GetContent(valuePairs));
            return true;
        }

        /// <summary>
        /// 结束战斗
        /// </summary>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> EndFight()
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("mode", "0");
            await Post<string>("Api/UserPK/EndJoin", GetContent(valuePairs));
            return true;
        }

        /// <summary>
        /// 获取PK结果
        /// </summary>
        /// <param name="arena"></param>
        /// <returns></returns>
        public async System.Threading.Tasks.Task<bool> GetPKResult(Arena arena)
        {

            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            valuePairs.Add("groupID", arena.GroupId);
            valuePairs.Add("mode", "0");
            var list = await Post<List<CombatResult>>("Api/UserPK/GetPKResult", GetContent(valuePairs));
            arena.Results = list;
            return true;
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
            return await Get<T>(requestUrl, pairs, 0);
        }

        private async Task<T> Get<T>(string requestUrl, IEnumerable<KeyValuePair<string, string>> pairs, int count) where T : class
        {
            HttpResponseMessage response = null;
            string result = "";
            try
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

                response = await httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseContentRead);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("网络访问异常");
                }

                result = await response.Content.ReadAsStringAsync();
                dynamic obj = JsonConvert.DeserializeObject(result);
                if (obj.state != "success")
                    throw new Exception("接口返回数据异常!");

                JsonSerializerSettings setting = new JsonSerializerSettings();
                setting.NullValueHandling = NullValueHandling.Ignore;
                T data = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj.data), setting);
                return data;
            }
            catch(Exception ex)
            {
                if(count < 3)
                {
                    return await Get<T>(requestUrl, pairs, ++count);
                }
                else
                {
                    
                    throw new TransportException(string.Format("GET Error:{0}. Http Code:{1}, Response: {2}", requestUrl, response.StatusCode.ToString(), result), ex);
                }
            }
        }

        private async Task<T> Post<T>(string requestUrl, HttpContent content) where T : class
        {
            return await Post<T>(requestUrl, content, 0);
        }

        private async Task<T> Post<T>(string requestUrl, HttpContent content, int count) where T : class
        {
            HttpResponseMessage response = null;
            string result = "";
            try
            {
                response = await httpClient.PostAsync(requestUrl, content);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("网络访问异常");
                }

                result = await response.Content.ReadAsStringAsync();
                dynamic obj = JsonConvert.DeserializeObject(result);
                if (obj.state != "success")
                {
                    return null;
                }

                T data = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj.data));
                return data;
            }
            catch(Exception ex)
            {
                if(count <3)
                {
                    return await Post<T>(requestUrl, content, ++count);
                }
                else
                {
                    throw new TransportException(string.Format("POST Error:{0}|{1}. Http Code: {2}, Response: {3}", requestUrl, JsonConvert.SerializeObject(content), response.StatusCode.ToString(), result), ex);
                }
            }
        }

    }

    public class TransportException: Exception
    {
        public TransportException(string errText) : base(errText )
        {

        }

        public TransportException(string errText, Exception innerException): base(errText,innerException)
        {

        }

        
    }
}
