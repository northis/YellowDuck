using System;
using System.Linq;
using System.Linq.Expressions;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces;

namespace YellowDuck.LearnChinese.Data
{
    public sealed class EfRepository : ILearnWordRepository
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


        public Word[] GetWords(Expression<Func<Word, bool>> whereCondition)
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

        public void EditWord(Word word)
        {
            using (var cntxt = _getContext())
            {
                var idWord = word.Id;
                var chineseWord = word.ChineseWord;
                var originalWord = cntxt.Words.FirstOrDefault(a => a.Id == idWord || a.ChineseWord == chineseWord);

                if (originalWord == null)
                    throw new Exception(
                        $"Правка невозможна, такого слова в хранилище нет. Id={idWord}, ChineseWord={chineseWord}");

                originalWord.PinyinWord = word.PinyinWord;
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

        public void AddWord(Word word)
        {
            using (var cntxt = _getContext())
            {
                var chineseWord = word.ChineseWord;
                var originalWord = cntxt.Words.FirstOrDefault(a => a.ChineseWord == chineseWord);

                if (originalWord != null)
                    throw new Exception($"Слово {chineseWord} уже есть в хранилище.");

                cntxt.Words.Add(word);
                cntxt.SaveChanges();
            }
        }

        public Score GetScore(long idUser, long idWord)
        {
            using (var cntxt = _getContext())
            {
                var score = cntxt.Scores.FirstOrDefault(a => a.IdUser == idUser && a.IdWord == idWord);

                if (score != null)
                    return score;

                var user = cntxt.Users.FirstOrDefault(a => a.IdUser == idUser);
                var word = cntxt.Words.FirstOrDefault(a => a.Id == idWord);

                if (user == null || word == null)
                    throw new Exception(
                        $"Ошибка при получении прогресса обучения. Аргументы недопустимы idUser={idUser}, idWord={idWord}");

                score = new Score {User = user, Word = word, LastView = GetRepositoryTime()};
                cntxt.Scores.Add(score);
                cntxt.SaveChanges();

                return score;
            }
        }

        public void SetPollMode(EPollModes pollMode, long userId, long wordId)
        {
            
        }

        public void SetScore(Score score)
        {
            using (var cntxt = _getContext())
            {
                var idScore = score.Id;
                var originalScore = cntxt.Scores.FirstOrDefault(a => a.Id == idScore);

                if (originalScore == null)
                    throw new Exception(
                        $"Обновление прогресса невозможно, такой сущности в хранилище нет. idScore={idScore}");

                originalScore.IsInLearnMode = score.IsInLearnMode;
                originalScore.LastLearned = score.LastLearned;
                originalScore.LastLearnMode = score.LastLearnMode;
                originalScore.LastView = score.LastView;

                originalScore.OriginalWordCount = score.OriginalWordCount;
                originalScore.OriginalWordSuccessCount = score.OriginalWordSuccessCount;
                originalScore.PronunciationCount = score.PronunciationCount;
                originalScore.PronunciationSuccessCount = score.PronunciationSuccessCount;
                originalScore.TranslationCount = score.TranslationCount;
                originalScore.TranslationSuccessCount = score.TranslationSuccessCount;

                originalScore.RightAnswerNumber = score.RightAnswerNumber;
                originalScore.ViewCount = score.ViewCount;

                cntxt.SaveChanges();
            }
        }

        public void AddUser(User user)
        {
            using (var cntxt = _getContext())
            {
                var idUser = user.IdUser;
                var originalUser = cntxt.Users.FirstOrDefault(a => a.IdUser == idUser);

                if (originalUser != null)
                    throw new Exception(
                        $"Добавление невозможно, такой пользователь уже существует. idUser={idUser}");

                cntxt.Users.Add(user);
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

        #endregion
    }
}
