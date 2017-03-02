using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YellowDuck.LearnChinese.Data;
using YellowDuck.LearnChinese.Data.Ef;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChinese.Providers;

namespace YellowDuck.LearnChineseBotService.Tests
{
    [TestClass]
    public class IntegrationDbTests
    {
        private const long IdTestUser = 0;
        private const long IdFriendTestUser = 1;

        private EfRepository GetCleanDb()
        {
            var cntxt = new EfRepository(new LearnChineseDbContext());

            using (var cn = GetDbContext())
            {
                cn.Scores.RemoveRange(cn.Scores);
                cn.Words.RemoveRange(cn.Words);
                cn.Users.RemoveRange(cn.Users);
                cn.UserSharings.RemoveRange(cn.UserSharings);
                cn.Users.Add(new User {IdUser = IdTestUser, Name = nameof(IdTestUser)});
                cn.Users.Add(new User {IdUser = IdFriendTestUser, Name = nameof(IdFriendTestUser)});
                cn.UserSharings.Add(new UserSharing {IdOwner = IdTestUser, IdFriend = IdFriendTestUser});
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
            IWordRepository iCntxt = cntxt;

            var chineseWord = "体育馆";
            iCntxt.AddWord(new Word
            {
                OriginalWord = chineseWord,
                LastModified = iCntxt.GetRepositoryTime(),
                Pronunciation = "tǐ|yù|guǎn",
                Translation = "Спортзал",
                CardAll = new byte[] {0x1, 0x2},
                IdOwner = IdTestUser
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
            IWordRepository iCntxt = cntxt;

            var chineseWord = "体育馆";
            using (var cn = GetDbContext())
            {
                cn.Words.Add(new Word
                {
                    OriginalWord = chineseWord,
                    LastModified = iCntxt.GetRepositoryTime(),
                    Pronunciation = "tǐyùguǎn",
                    Translation = "Спортзал",
                    IdOwner = IdTestUser
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
            IWordRepository iCntxt = cntxt;

            var chineseWord = "体育馆";
            var word = new Word
            {
                OriginalWord = chineseWord,
                LastModified = iCntxt.GetRepositoryTime(),
                Pronunciation = "tǐyùguǎn",
                Translation = "СпортзалOld",
                IdOwner = IdTestUser
            };

            var prov = MainLogicTests.GetChineseWordParseProvider();
            var grn = new WpfFlashCardGenerator(prov);

            using (var cn = GetDbContext())
            {
                cn.Words.Add(word);
                cn.SaveChanges();

                var guid = Guid.NewGuid().ToString();
                var dt = iCntxt.GetRepositoryTime();
                word.LastModified = dt;

                word.Usage = guid;

                word.Translation = "Спортзал";
                word.Pronunciation = "tǐ|yù|guǎn";

                word.CardAll = grn.Generate(word, ELearnMode.FullView);
                word.CardOriginalWord = grn.Generate(word, ELearnMode.OriginalWord);
                word.CardPronunciation = grn.Generate(word, ELearnMode.Pronunciation);
                word.CardTranslation = grn.Generate(word, ELearnMode.Translation);

                iCntxt.EditWord(word);

                var newWord = cn.Words.FirstOrDefault(a => a.OriginalWord == word.OriginalWord);
                Assert.IsNotNull(newWord);
                Assert.IsNotNull(newWord.CardAll);
                Assert.IsNotNull(newWord.CardOriginalWord);
                Assert.IsNotNull(newWord.CardPronunciation);
                Assert.IsNotNull(newWord.CardTranslation);
                
                //System.IO.File.WriteAllBytes($@"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}\CardAll.png", newWord.CardAll);
                //System.IO.File.WriteAllBytes($@"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}\CardOriginalWord.png", newWord.CardOriginalWord);
                //System.IO.File.WriteAllBytes($@"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}\CardPronunciation.png", newWord.CardPronunciation);
                //System.IO.File.WriteAllBytes($@"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}\CardTranslation.png", newWord.CardTranslation);


                Assert.AreEqual("tǐ|yù|guǎn", newWord.Pronunciation);
                Assert.AreEqual("Спортзал", newWord.Translation);
                Assert.AreEqual(guid, newWord.Usage);
                Assert.AreEqual(dt, newWord.LastModified);
            }
        }

        [TestMethod]
        public void LearnWordTest()
        {
            var cntxt = GetCleanDb();
            IWordRepository iCntxt = cntxt;
            var iStudyProvider = new ClassicStudyProvider(iCntxt);
            
            var idUser = IdFriendTestUser;

            var words = new[]
            {
                new Word
                {
                    OriginalWord = "句子",
                    Pronunciation = "jù|zi",
                    Translation = "предложение; фраза",
                IdOwner = IdTestUser
                },
                new Word
                {
                    OriginalWord = "够了",
                    Pronunciation = "gòu|le",
                    Translation = "довольно; хватит",
                IdOwner = IdTestUser
                },
                new Word
                {
                    OriginalWord = "收",
                    Pronunciation = "shōu",
                    Translation = "получать",
                IdOwner = IdTestUser
                },
                new Word
                {
                    OriginalWord = "接送",
                    Pronunciation = "jiē|sòng",
                    Translation = "забирать",
                IdOwner = IdTestUser
                },
                new Word
                {
                    OriginalWord = "路",
                    
                    Pronunciation = "lù",
                    Translation = "дорога, улица",
                IdOwner = IdTestUser
                }
            };
            

            using (var cn = GetDbContext())
            {

                foreach (var word in words)
                {
                    cn.Words.Add(word);
                    word.LastModified = iCntxt.GetRepositoryTime();
                    cn.SaveChanges();
                }
 
                var lastIndex = words.Length - 1;
                var leftAnswersCount = lastIndex;
                //var firstIndex = 0;
               
                var score = iStudyProvider.LearnWord(idUser, ELearnMode.OriginalWord,
                    EGettingWordsStrategy.NewFirst);
                Assert.IsNotNull(score);
                //Assert.IsNotNull(score.WordToCheck.OriginalWord == words[lastIndex].OriginalWord);


                score = iStudyProvider.LearnWord(idUser, ELearnMode.OriginalWord,
                    EGettingWordsStrategy.NewMostDifficult);
                Assert.IsNotNull(score);
               // Assert.IsNotNull(score.WordToCheck.OriginalWord == words[lastIndex].OriginalWord);
                

                score = iStudyProvider.LearnWord(idUser, ELearnMode.OriginalWord,
                    EGettingWordsStrategy.Random);
                Assert.IsNotNull(score);
                

                score = iStudyProvider.LearnWord(idUser, ELearnMode.OriginalWord,
                    EGettingWordsStrategy.OldFirst);
                Assert.IsNotNull(score);
               // Assert.IsNotNull(score.WordToCheck.OriginalWord == words[firstIndex].OriginalWord);
                

                score = iStudyProvider.LearnWord(idUser, ELearnMode.OriginalWord,
                    EGettingWordsStrategy.OldMostDifficult);
                Assert.IsNotNull(score);
                //Assert.IsNotNull(score.WordToCheck.OriginalWord == words[firstIndex].OriginalWord);


                //var scoreNew = score;
                //var intersectedRows = score.Intersect(
                //    words.Where(a => a.OriginalWord != scoreNew.WordToCheck.OriginalWord).Select(a => a.OriginalWord));
                //Assert.IsTrue(intersectedRows.Count() == leftAnswersCount);

                //score = iCntxt.LearnWord(user.IdUser, ELearnMode.Pronunciation,
                //    EGettingWordsStrategy.NewFirst);
                //intersectedRows = score.Answers.Intersect(
                //    words.Where(a => a.OriginalWord != scoreNew.WordToCheck.OriginalWord).Select(a => a.Pronunciation));
                //Assert.IsTrue(intersectedRows.Count() == leftAnswersCount);

                //score = iCntxt.LearnWord(user.IdUser, ELearnMode.Translation,
                //    EGettingWordsStrategy.NewFirst);
                //intersectedRows = score.Answers.Intersect(
                //    words.Where(a => a.OriginalWord != scoreNew.WordToCheck.OriginalWord).Select(a => a.Translation));
                //Assert.IsTrue(intersectedRows.Count() == leftAnswersCount);

                //score = iCntxt.LearnWord(user.IdUser, ELearnMode.FullView,
                //    EGettingWordsStrategy.NewFirst);
                //Assert.AreEqual(0, score.Answers.Length);
            }
        }

        [TestMethod]
        public void UserAddRemoveTest()
        {
            var cntxt = GetDbContext();
            IWordRepository iCntxt = GetCleanDb();

            var idUser = 100;
            var user = new User {IdUser = idUser, Name = string.Empty};
            iCntxt.AddUser(user);

            Assert.IsTrue(cntxt.Users.Any(a => a.IdUser == idUser));

            iCntxt.RemoveUser(idUser);

            Assert.IsFalse(cntxt.Users.Any(a => a.IdUser == idUser));
        }
    }
}
