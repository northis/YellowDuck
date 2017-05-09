using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YellowDuck.LearnChineseBotService.MainExecution;
// ReSharper disable AccessToModifiedClosure

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

            var output = QueryHandler.GetNoEmojiString(input);

            Assert.IsTrue(output == "yes老师");
        }

        [TestMethod]
        public void AntiDdosCheckerTest()
        {
            var baseDt = DateTime.Now;
            var userId = 0;
            var operationDuration = TimeSpan.FromSeconds(3);
            
            var checker = new AntiDdosChecker(() => baseDt);
            var smallTs = checker.FrequencyThreshold.Add(TimeSpan.FromMilliseconds(-1));
            var bigTs = checker.BanInterval.Add(TimeSpan.FromMilliseconds(-1));

            Assert.IsTrue(checker.AllowUser(userId));

            baseDt = baseDt.Add(operationDuration);
            checker.UserQueryProcessed(userId);

            baseDt = baseDt.Add(smallTs.Add(TimeSpan.FromMilliseconds(2)));
            Assert.IsTrue(checker.AllowUser(userId));

            baseDt = baseDt.Add(smallTs);
            Assert.IsFalse(checker.AllowUser(userId));

            baseDt = baseDt.Add(bigTs);
            Assert.IsFalse(checker.AllowUser(userId));

            baseDt = baseDt.Add(TimeSpan.FromMilliseconds(2));
            Assert.IsTrue(checker.AllowUser(userId));
        }
    }
}
