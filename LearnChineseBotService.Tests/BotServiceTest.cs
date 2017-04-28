using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YellowDuck.LearnChineseBotService.MainExecution;

namespace LearnChineseBotService.Tests
{
    [TestClass]
    public class BotServiceTest
    {
        [TestMethod]
        public void EmojiRemovingTest()
        {
            var input = "➕yes老师";

            var cats = new List<UnicodeCategory>();
            foreach (var inp in input)
            {
                cats.Add(char.GetUnicodeCategory(inp));
            }

            var output = PollWorker.GetNoEmojiString(input);

            Assert.IsTrue(output == "yes老师");
        }
    }
}
