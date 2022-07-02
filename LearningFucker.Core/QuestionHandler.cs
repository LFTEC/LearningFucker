using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using LearningFucker.Models;
using System.Threading.Tasks;
using System.Net.Http;
using Polly;
using Newtonsoft.Json;

namespace LearningFucker
{
    public class QuestionHandler
    {
        public QuestionHandler()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://server.jcdev.cc:52126");            
        }

        private HttpClient httpClient;

        public async Task<Question> GetRow(int tmid)
        {
            var delayRetry = Policy.Handle<HttpRequestException>().Or<TaskCanceledException>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(3)
                });

            return await delayRetry.ExecuteAsync<Question>(async () =>
            {
                return await Get(tmid);
            });
        }

        public async System.Threading.Tasks.Task WriteData(Question question)
        {
            var delayRetry = Policy.Handle<HttpRequestException>().Or<TaskCanceledException>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(3)
                });

            await delayRetry.ExecuteAsync(async () =>
            {
                await Write(question);
            });
        }

        private async Task<Question> Get(int tmid)
        {
            HttpResponseMessage response = null;
            string result = "";
            string url = $"/get?id={tmid}";

            response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("网络访问异常");
            }

            result = await response.Content.ReadAsStringAsync();
            Question obj = JsonConvert.DeserializeObject<Question>(result);

            return obj;
        }

        private async System.Threading.Tasks.Task Write(Question question)
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(question),Encoding.UTF8,"application/json");

            HttpResponseMessage response = null;

            response = await httpClient.PostAsync("/write", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("网络访问异常, return status code: " + response.StatusCode);
            }

        }

    }
}
