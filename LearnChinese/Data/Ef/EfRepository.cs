using System;
using System.Linq;
using System.Linq.Expressions;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Data.Ef
{
    public sealed class EfRepository : IWordRepository
    {
        #region Fields

        private readonly Func<LearnChineseDbContext> _getContext;

        #endregion

        #region Constructors

        public EfRepository(Func<LearnChineseDbContext> getContext)
        {
            _getContext = getContext;
        }

        #endregion

        #region Methods


        public IWord[] GetWords(Expression<Func<IWord, bool>> whereCondition)
        {
            using (var cntxt = _getContext())
            {
                return cntxt.Words.Where(whereCondition).ToArray();
            }
        }

        public DateTime GetRepositoryTime()
        {
            using (var cntxt = _getContext())
            {
                return cntxt.Database.SqlQuery<DateTime>("select getdate()", "").FirstOrDefault();
            }
        }

        public void EditWord(IWord word)
        {
            using (var cntxt = _getContext())
            {
                var idWord = word.Id;
                var chineseWord = word.OriginalWord;
                var originalWord = cntxt.Words.FirstOrDefault(a => a.Id == idWord || a.OriginalWord == chineseWord);

                if (originalWord == null)
                    throw new Exception(
                        $"Правка невозможна, такого слова в хранилище нет. Id={idWord}, ChineseWord={chineseWord}");

                originalWord.Pronunciation = word.Pronunciation;
                originalWord.Translation = word.Translation;

                cntxt.SaveChanges();
            }
        }

        public void DeleteWord(long wordId)
        {
            using (var cntxt = _getContext())
            {
                var originalWord = cntxt.Words.FirstOrDefault(a => a.Id == wordId);

                if (originalWord == null)
                    throw new Exception(
                        $"Удаление невозможно, такого слова в хранилище нет. Id={wordId}");

                cntxt.Words.Remove(originalWord);
                cntxt.SaveChanges();
            }
        }

        public void AddWord(IWord word)
        {
            using (var cntxt = _getContext())
            {
                var chineseWord = word.OriginalWord;
                var originalWord = cntxt.Words.FirstOrDefault(a => a.OriginalWord == chineseWord);

                if (originalWord != null)
                    throw new Exception($"Слово {chineseWord} уже есть в хранилище.");

                cntxt.Words.Add(new Word
                {
                    CardAll = word.CardAll,
                    CardOriginalWord = word.CardOriginalWord,
                    CardPronunciation = word.CardPronunciation,
                    CardTranslation = word.CardTranslation,
                    LastModified = GetRepositoryTime(),
                    OriginalWord = word.OriginalWord,
                    Pronunciation = word.Pronunciation,
                    Translation = word.Translation,
                    Usage = word.Usage,
                    Id = word.Id
                });
                cntxt.SaveChanges();
            }
        }
        

        public void AddUser(IUser user)
        {
            using (var cntxt = _getContext())
            {
                var idUser = user.IdUser;
                var originalUser = cntxt.Users.FirstOrDefault(a => a.IdUser == idUser);

                if (originalUser != null)
                    throw new Exception(
                        $"Добавление невозможно, такой пользователь уже существует. idUser={idUser}");

                cntxt.Users.Add(new User {IdUser = user.IdUser, Name = user.Name});
                cntxt.SaveChanges();
            }
        }

        public void RemoveUser(long userId)
        {
            using (var cntxt = _getContext())
            {
                var originalUser = cntxt.Users.FirstOrDefault(a => a.IdUser == userId);

                if (originalUser == null)
                    throw new Exception(
                        $"Удаление невозможно, такого пользователя и так не существует. userId={userId}");

                cntxt.Users.Remove(originalUser);
                cntxt.SaveChanges();
            }
        }

        public void AddFriendUser(long ownerUserId, long friendUserId)
        {
            using (var cntxt = _getContext())
            {
                var ownerUser = cntxt.Users.FirstOrDefault(a => a.IdUser == ownerUserId);

                if (ownerUser == null)
                    throw new Exception(
                        $"Поделиться списком слов невозможно. ownerUserId={ownerUserId}");


                var friendUser = cntxt.Users.FirstOrDefault(a => a.IdUser == friendUserId);
                if (friendUser == null)
                    throw new Exception(
                        $"Поделиться списком слов невозможно. friendUserId={friendUserId}");

                ownerUser.OwnerUserSharings.Add(new UserSharing {UserFriend = friendUser});
                cntxt.SaveChanges();
            }
        }

        public void RemoveFriendUser(long ownerUserId, long friendUserId)
        {
            using (var cntxt = _getContext())
            {
                var ownerUser = cntxt.Users.FirstOrDefault(a => a.IdUser == ownerUserId);

                if (ownerUser == null)
                    throw new Exception(
                        $"Забрать доступ к своему словарю невозможно. ownerUserId={ownerUserId}");

                var friendUser = cntxt.Users.FirstOrDefault(a => a.IdUser == friendUserId);
                if (friendUser == null)
                    throw new Exception(
                        $"Забрать доступ к своему словарю невозможно. friendUserId={friendUserId}");

                ownerUser.OwnerUserSharings.Remove(ownerUser.OwnerUserSharings.First(a => a.UserFriend == friendUser));
                cntxt.SaveChanges();
            }
        }

        #endregion
    }
}
