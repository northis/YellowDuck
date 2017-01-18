using System;
using System.Linq;
using System.Linq.Expressions;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Data.Ef
{
    public sealed class EfRepository : ILearnWordRepository
    {
        #region Fields

        private readonly Func<LearnChineseDbContext> _getContext;

        public const ushort PollAnswersCount = 4;

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

        Score GetScore(long idUser, long idWord)
        {
            using (var cntxt = _getContext())
            {
                var score = cntxt.Scores.FirstOrDefault(a => a.IdUser == idUser && a.IdWord == idWord);

                if (score != null)
                    return score;

                var user = cntxt.Users.FirstOrDefault(a => a.IdUser == idUser);
                var word = cntxt.Words.FirstOrDefault(a => a.Id == idWord);

                if (user == null || word == null)
                    return null; 

                score = new Score {User = user, Word = word, LastView = GetRepositoryTime()};
                cntxt.Scores.Add(score);
                cntxt.SaveChanges();

                return score;
            }
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
        

        private IQueryable<Score> GetDifficultScores(ELearnMode learnMode, IQueryable<Score> scores)
        {
            switch (learnMode)
            {
                case ELearnMode.OriginalWord:
                    return
                        scores.OrderByDescending(
                            a => a.OriginalWordCount > 0 ? a.OriginalWordSuccessCount/a.OriginalWordCount : 0);

                case ELearnMode.Pronunciation:
                    return
                        scores.OrderByDescending(
                            a => a.PronunciationCount > 0 ? a.PronunciationSuccessCount/a.PronunciationCount : 0);

                case ELearnMode.Translation:
                    return
                        scores.OrderByDescending(
                            a => a.TranslationCount > 0 ? a.TranslationSuccessCount/a.TranslationCount : 0);
            }

            return scores;
        }

        private IQueryable<WordDatesView> GetWordDates(LearnChineseDbContext cntxt, IQueryable<Score> scores)
        {
            return cntxt.Words.GroupJoin(scores, w => w.Id, s => s.IdWord, (w, s) => new {w, s})
                .SelectMany(a => a.s.DefaultIfEmpty(),
                    (a, b) =>
                        new WordDatesView
                        {
                            Word = a.w,
                            LastLearned = b != null ? b.LastLearned : null,
                            LastView = b != null ? (DateTime?) b.LastView : null
                        });
        }


        public Poll LearnWord(long userId, ELearnMode learnMode, EGettingWordsStrategy strategy)
        {
            IWord word;

            using (var cntxt = _getContext())
            {
                var scores = cntxt.Scores.Where(a => a.IdUser == userId);
                var difficultScores = GetDifficultScores(learnMode, scores);

                var wordScoresLeftJoin = GetWordDates(cntxt, scores);
                var wordDifficultScoresLeftJoin = GetWordDates(cntxt, difficultScores);

                IQueryable<IWord> userWords;
                switch (strategy)
                {
                    case EGettingWordsStrategy.NewFirst:

                        userWords = wordScoresLeftJoin.OrderByDescending(a => a.LastLearned)
                            .ThenByDescending(a => a.LastView)
                            .ThenByDescending(a => a.Word.LastModified)
                            .Select(a => a.Word);
                        break;

                    case EGettingWordsStrategy.NewMostDifficult:

                        userWords = wordDifficultScoresLeftJoin.OrderByDescending(a => a.LastLearned)
                            .ThenByDescending(a => a.LastView)
                            .ThenByDescending(a => a.Word.LastModified)
                            .Select(a => a.Word);
                        break;

                    case EGettingWordsStrategy.OldFirst:

                        userWords = wordScoresLeftJoin.OrderBy(a => a.LastLearned)
                            .ThenBy(a => a.LastView)
                            .ThenBy(a => a.Word.LastModified)
                            .Select(a => a.Word);
                        break;

                    case EGettingWordsStrategy.OldMostDifficult:

                        userWords = wordDifficultScoresLeftJoin.OrderBy(a => a.LastLearned)
                            .ThenBy(a => a.LastView)
                            .ThenBy(a => a.Word.LastModified)
                            .Select(a => a.Word);
                        break;

                    case EGettingWordsStrategy.Random:
                        
                        userWords = cntxt.Words.OrderBy(a => Guid.NewGuid());
                        break;

                    default:
                        userWords = scores.Select(a => a.Word);
                        break;

                }

                word = userWords.FirstOrDefault();

                if (word == null)
                    throw new Exception($"Нет подходящих слов для изучения. userId={userId}");

                var wordId = word.Id;
                var score = GetScore(userId, wordId);

                score.LastLearnMode = learnMode.ToString();
                score.IsInLearnMode = learnMode != ELearnMode.FullView;
                score.LastLearned = GetRepositoryTime();

                var answers = userWords.Where(a => a.Id != word.Id).OrderBy(a => Guid.NewGuid()).Take(PollAnswersCount);

                string[] stringAnswers = null;
                
                switch (learnMode)
                {
                    case ELearnMode.OriginalWord:
                        score.OriginalWordCount++;
                        stringAnswers = answers.Select(a => a.OriginalWord).ToArray();
                        break;
                        
                    case ELearnMode.Pronunciation:
                        score.PronunciationCount++;
                        stringAnswers = answers.Select(a => a.Pronunciation).ToArray();
                        break;

                    case ELearnMode.Translation:
                        score.TranslationCount++;
                        stringAnswers = answers.Select(a => a.Translation).ToArray();
                        break;

                    case ELearnMode.FullView:
                        score.ViewCount++;
                        stringAnswers = new string[0];
                        break;
                }

                cntxt.SaveChanges();

                return new Poll(stringAnswers, word);
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

        #endregion
    }
}
