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
                TmBaseTx = "�ж���",
                TmTxStr = "�ж���",
                Title = "��װ�豸������Ա��ָ����Ա��������Ч�������豸��ҵ��Ա֤�ſ�����ҵ��",
                TmKey = "��װ��ҵ",
                Options = "��;��",
                Answers = "A",
                Difficulty = "����",
                Score = 1.0m,
                UserAnswer = "",
                Remark = ""
            };
            var questionHandler = new QuestionHandler();
            questionHandler.WriteData(question).Wait();
        }
    }
}