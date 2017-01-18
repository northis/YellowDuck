using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YellowDuck.LearnChinese.Data;
using YellowDuck.LearnChinese.Data.Ef;
using YellowDuck.LearnChinese.Interfaces;

namespace YellowDuck.LearnChineseBotService.Tests
{
    [TestClass]
    public class IntegrationDbTests
    {
        private EfRepository GetCleanDb()
        {
            var cntxt = new EfRepository(GetDbContext);

            using (var cn = GetDbContext())
            {
                cn.Scores.RemoveRange(cn.Scores);
                cn.Words.RemoveRange(cn.Words);
                cn.Users.RemoveRange(cn.Users);
                cn.SaveChanges();
            }

            return cntxt;
        }

        private LearnChineseDbContext GetDbContext()
        {
            return new LearnChineseDbContext();
        }

        [TestMethod]
        public void AddWordTest()
        {
            var cntxt = GetCleanDb();
            ILearnWordRepository iCntxt = cntxt;

            var chineseWord = "体育馆";
            iCntxt.AddWord(new Word
            {
                OriginalWord = chineseWord,
                LastModified = iCntxt.GetRepositoryTime(),
                Pronunciation = "tǐyùguǎn",
                Translation = "Спортзал",
                CardAll = new byte[] {0x1, 0x2}
            });

            using (var cn = GetDbContext())
            {
                var word = cn.Words.FirstOrDefault(a => a.OriginalWord == chineseWord);
                Assert.IsNotNull(word);

                Assert.IsNotNull(word.CardAll);
            }
        }

        [TestMethod]
        public void RemoveWordFromRepTest()
        {
            var cntxt = GetCleanDb();
            ILearnWordRepository iCntxt = cntxt;

            var chineseWord = "体育馆";
            using (var cn = GetDbContext())
            {
                cn.Words.Add(new Word
                {
                    OriginalWord = chineseWord,
                    LastModified = iCntxt.GetRepositoryTime(),
                    Pronunciation = "tǐyùguǎn",
                    Translation = "Спортзал"
                });
                cn.SaveChanges();
            }

            using (var cn = GetDbContext())
            {
                var wordInDb = cn.Words.FirstOrDefault(a => a.OriginalWord == chineseWord);
                Assert.IsNotNull(wordInDb);

                iCntxt.DeleteWord(wordInDb.Id);


                Assert.IsFalse(cn.Words.Any(a => a.OriginalWord == chineseWord));
            }

        }

        [TestMethod]
        public void EditWordTest()
        {
            var cntxt = GetCleanDb();
            ILearnWordRepository iCntxt = cntxt;

            var chineseWord = "体育馆";
            var word = (new Word
            {
                OriginalWord = chineseWord,
                LastModified = iCntxt.GetRepositoryTime(),
                Pronunciation = "tǐyùguǎn",
                Translation = "Спортзал"
            });

            using (var cn = GetDbContext())
            {
                cn.Words.Add(word);
                cn.SaveChanges();

                var guid = Guid.NewGuid().ToString();
                var dt = iCntxt.GetRepositoryTime();
                word.LastModified = dt;

                word.Usage = word.Translation = word.Pronunciation = guid;

                iCntxt.EditWord(word);

                var newWord = cn.Words.FirstOrDefault(a => a.OriginalWord == word.OriginalWord);
                Assert.IsNotNull(newWord);
                Assert.AreEqual(guid, newWord.Pronunciation);
                Assert.AreEqual(guid, newWord.Translation);
                Assert.AreEqual(guid, newWord.Usage);
                Assert.AreEqual(dt, newWord.LastModified);
            }
        }

        [TestMethod]
        public void LearnWordTest()
        {
            var cntxt = GetCleanDb();
            ILearnWordRepository iCntxt = cntxt;

            var idUser = 10;
            var user = new User
            {
                IdUser = idUser,
                Name = string.Empty
            };

            var words = new[]
            {
                new Word
                {
                    OriginalWord = "句子",
                    Pronunciation = "jùzi",
                    Translation = "предложение; фраза"
                },
                new Word
                {
                    OriginalWord = "够了",
                    Pronunciation = "gòu le",
                    Translation = "довольно; хватит"
                },
                new Word
                {
                    OriginalWord = "收",
                    Pronunciation = "shōu",
                    Translation = "получать"
                },
                new Word
                {
                    OriginalWord = "接送",
                    Pronunciation = "jiēsòng",
                    Translation = "забирать"
                },
                new Word
                {
                    OriginalWord = "路",
                    
                    Pronunciation = "lù",
                    Translation = "дорога, улица"
                }
            };
            

            using (var cn = GetDbContext())
            {
                cn.Users.Add(user);
                cn.SaveChanges();

                foreach (var word in words)
                {
                    cn.Words.Add(word);
                    word.LastModified = iCntxt.GetRepositoryTime();
                    cn.SaveChanges();
                }

                var lastIndex = words.Length - 1;
                var leftAnswersCount = lastIndex;
                var firstIndex = 0;
                
                var score = iCntxt.LearnWord(user.IdUser, LearnChinese.Enums.ELearnMode.OriginalWord,
                    LearnChinese.Enums.EGettingWordsStrategy.NewFirst);
                Assert.IsNotNull(score);
                Assert.IsNotNull(score.WordToCheck.OriginalWord == words[lastIndex].OriginalWord);


                score = iCntxt.LearnWord(user.IdUser, LearnChinese.Enums.ELearnMode.OriginalWord,
                    LearnChinese.Enums.EGettingWordsStrategy.NewMostDifficult);
                Assert.IsNotNull(score);
                Assert.IsNotNull(score.WordToCheck.OriginalWord == words[lastIndex].OriginalWord);
                

                score = iCntxt.LearnWord(user.IdUser, LearnChinese.Enums.ELearnMode.OriginalWord,
                    LearnChinese.Enums.EGettingWordsStrategy.Random);
                Assert.IsNotNull(score);
                

                score = iCntxt.LearnWord(user.IdUser, LearnChinese.Enums.ELearnMode.OriginalWord,
                    LearnChinese.Enums.EGettingWordsStrategy.OldFirst);
                Assert.IsNotNull(score);
                Assert.IsNotNull(score.WordToCheck.OriginalWord == words[firstIndex].OriginalWord);
                

                score = iCntxt.LearnWord(user.IdUser, LearnChinese.Enums.ELearnMode.OriginalWord,
                    LearnChinese.Enums.EGettingWordsStrategy.OldMostDifficult);
                Assert.IsNotNull(score);
                Assert.IsNotNull(score.WordToCheck.OriginalWord == words[firstIndex].OriginalWord);


                var scoreNew = score;
                var intersectedRows = score.Answers.Intersect(
                    words.Where(a => a.OriginalWord != scoreNew.WordToCheck.OriginalWord).Select(a => a.OriginalWord));
                Assert.IsTrue(intersectedRows.Count() == leftAnswersCount);

                score = iCntxt.LearnWord(user.IdUser, LearnChinese.Enums.ELearnMode.Pronunciation,
                    LearnChinese.Enums.EGettingWordsStrategy.NewFirst);
                intersectedRows = score.Answers.Intersect(
                    words.Where(a => a.OriginalWord != scoreNew.WordToCheck.OriginalWord).Select(a => a.Pronunciation));
                Assert.IsTrue(intersectedRows.Count() == leftAnswersCount);

                score = iCntxt.LearnWord(user.IdUser, LearnChinese.Enums.ELearnMode.Translation,
                    LearnChinese.Enums.EGettingWordsStrategy.NewFirst);
                intersectedRows = score.Answers.Intersect(
                    words.Where(a => a.OriginalWord != scoreNew.WordToCheck.OriginalWord).Select(a => a.Translation));
                Assert.IsTrue(intersectedRows.Count() == leftAnswersCount);

                score = iCntxt.LearnWord(user.IdUser, LearnChinese.Enums.ELearnMode.FullView,
                    LearnChinese.Enums.EGettingWordsStrategy.NewFirst);
                Assert.AreEqual(0, score.Answers.Length);
            }
        }

        [TestMethod]
        public void UserAddRemoveTest()
        {
            var cntxt = GetDbContext();
            ILearnWordRepository iCntxt = GetCleanDb();

            var idUser = 100;
            var user = new User {IdUser = idUser, Name = string.Empty};
            iCntxt.AddUser(user);

            Assert.IsTrue(cntxt.Users.Any(a => a.IdUser == idUser));

            iCntxt.RemoveUser(idUser);

            Assert.IsFalse(cntxt.Users.Any(a => a.IdUser == idUser));
        }
    }
}
