using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YellowDuck.LearnChinese.Data;
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
                cn.Words.RemoveRange(cn.Words);
                cn.SaveChanges();
            }

            return cntxt;
        }

        private LearnChineseDbContext GetDbContext()
        {
            return new LearnChineseDbContext();
        }

        [TestMethod]
        public void AddWordToRepTest()
        {
            var cntxt = GetCleanDb();
            ILearnWordRepository iCntxt = cntxt;

            var chineseWord = "体育馆";
            iCntxt.AddWord(new Word
            {
                ChineseWord = chineseWord,
                LastModified = iCntxt.GetRepositoryTime(),
                PinyinWord = "tǐyùguǎn",
                Translation = "Спортзал"
            });

            using (var cn = GetDbContext())
            {
                Assert.IsTrue(cn.Words.Any(a => a.ChineseWord == chineseWord));
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
                    ChineseWord = chineseWord,
                    LastModified = iCntxt.GetRepositoryTime(),
                    PinyinWord = "tǐyùguǎn",
                    Translation = "Спортзал"
                });
                cn.SaveChanges();
            }

            using (var cn = GetDbContext())
            {
                var wordInDb = cn.Words.FirstOrDefault(a => a.ChineseWord == chineseWord);
                Assert.IsNotNull(wordInDb);

                iCntxt.DeleteWord(wordInDb.Id);


                Assert.IsFalse(cn.Words.Any(a => a.ChineseWord == chineseWord));
            }

        }

        [TestMethod]
        public void EditWordInRepTest()
        {
            var cntxt = GetCleanDb();
            ILearnWordRepository iCntxt = cntxt;

            var chineseWord = "体育馆";
            var word = (new Word
            {
                ChineseWord = chineseWord,
                LastModified = iCntxt.GetRepositoryTime(),
                PinyinWord = "tǐyùguǎn",
                Translation = "Спортзал"
            });

            using (var cn = GetDbContext())
            {
                cn.Words.Add(word);
                cn.SaveChanges();


                var guid = Guid.NewGuid().ToString();
                word.Translation = word.PinyinWord = guid;

                iCntxt.EditWord(word);

                var newWord = cn.Words.FirstOrDefault(a => a.ChineseWord == word.ChineseWord);
                Assert.IsNotNull(newWord);
                Assert.AreEqual(guid, newWord.PinyinWord);
                Assert.AreEqual(guid, newWord.Translation);
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
