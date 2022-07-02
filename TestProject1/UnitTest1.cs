using Microsoft.VisualStudio.TestTools.UnitTesting;
using LearningFucker;
using LearningFucker.Models;


namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void QuestionHandlerTest()
        {
            var questionHandler = new QuestionHandler();
            var data = questionHandler.GetRow(1804).Result;
            Assert.IsNotNull(data);
        }

        [TestMethod()]
        public void WriteDataTest()
        {
            var question = new Question
            {
                TmID = 1804,
                TkID = 0,
                TmSourceType = 0,
                TmTx = 593,
                TmBaseTx = "判断类",
                TmTxStr = "判断题",
                Title = "吊装设备操作人员和指挥人员必须获得有效的特种设备作业人员证才可以作业。",
                TmKey = "吊装作业",
                Options = "对;错",
                Answers = "A",
                Difficulty = "较易",
                Score = 1.0m,
                UserAnswer = "",
                Remark = ""
            };
            var questionHandler = new QuestionHandler();
            questionHandler.WriteData(question).Wait();
        }
    }
}