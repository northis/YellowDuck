using System;
using System.Linq;
using System.Linq.Expressions;
using YellowDuck.LearnChinese.Data.DbViews;
using YellowDuck.LearnChinese.Data.ObjectModels;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Extentions;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Data.Ef
{
    public sealed class EfRepository : IWordRepository
    {
        #region Fields

        private readonly LearnChineseDbContext _context;

        #endregion

        #region Constructors

        public EfRepository(LearnChineseDbContext context)
        {
            _context = context;
        }

        #endregion

        #region Methods


        public IWord[] GetWords(Expression<Func<IWord, bool>> whereCondition)
        {
            return _context.Words.Where(whereCondition).ToArray();
        }

        public DateTime GetRepositoryTime()
        {
            return _context.Database.SqlQuery<DateTime>("select getdate()", "").FirstOrDefault();
        }
        
        public IQueryable<IScore> GetDifficultScores(ELearnMode learnMode, IQueryable<IScore> scores)
        {
            switch (learnMode)
            {
                case ELearnMode.OriginalWord:
                    return
                        scores.OrderByDescending(
                            a => a.OriginalWordCount > 0 ? a.OriginalWordSuccessCount / a.OriginalWordCount : 0);

                case ELearnMode.Pronunciation:
                    return
                        scores.OrderByDescending(
                            a => a.PronunciationCount > 0 ? a.PronunciationSuccessCount / a.PronunciationCount : 0);

                case ELearnMode.Translation:
                    return
                        scores.OrderByDescending(
                            a => a.TranslationCount > 0 ? a.TranslationSuccessCount / a.TranslationCount : 0);
            }

            return scores;
        }



        public LearnUnit GetNextWord(WordSettings settings)
        {
            var userId = settings.UserId;
            var learnMode = settings.LearnMode;
            var strategy = settings.Strategy;
            var pollAnswersCount = settings.PollAnswersCount;

            var scores = _context.Scores.Where(a => a.IdUser == userId);
            
            var difficultScores = GetDifficultScores(learnMode, scores);

            var userAllowedWords =
                _context.Words.Where(a => a.UserOwner.OwnerUserSharings.Any(b => b.IdFriend == userId));

            var wordScoresLeftJoin = GetWordDates(userAllowedWords, scores);
            var wordDifficultScoresLeftJoin = GetWordDates(userAllowedWords, difficultScores);

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

                    userWords = _context.Words.OrderBy(a => Guid.NewGuid());
                    break;

                default:
                    userWords = scores.Select(a => a.Word);
                    break;

            }

            var word = userWords.FirstOrDefault();

            if (word == null)
                throw new Exception($"Нет подходящих слов для изучения. userId={userId}");

            var wordId = word.Id;
            var score = GetScore(userId, wordId);

            score.LastLearnMode = learnMode.ToString();
            score.IsInLearnMode = learnMode != ELearnMode.FullView;
            score.LastLearned = GetRepositoryTime();

            var answers = userWords.Where(a => a.Id != word.Id).OrderBy(a => Guid.NewGuid()).Take(pollAnswersCount);
            var questionItem = new LearnUnit();

            switch (learnMode)
            {
                case ELearnMode.OriginalWord:
                    score.OriginalWordCount++;
                    questionItem.Options = answers.Select(a => a.OriginalWord).ToArray();
                    questionItem.Picture = word.CardOriginalWord;
                    break;

                case ELearnMode.Pronunciation:
                    score.PronunciationCount++;
                    questionItem.Options = answers.Select(a => a.Pronunciation).ToArray();
                    questionItem.Picture = word.CardPronunciation;
                    break;

                case ELearnMode.Translation:
                    score.TranslationCount++;
                    questionItem.Options = answers.Select(a => a.Translation).ToArray();
                    questionItem.Picture = word.CardTranslation;
                    break;

                case ELearnMode.FullView:
                    score.ViewCount++;
                    questionItem.Options = new string[0];
                    questionItem.Picture = word.CardAll;
                    break;
            }

            _context.SaveChanges();

            return questionItem;
        }

        public Score GetScore(long idUser, long idWord)
        {
            var score = _context.Scores.FirstOrDefault(a => a.IdUser == idUser && a.IdWord == idWord);

            if (score != null)
                return score;

            var user = _context.Users.FirstOrDefault(a => a.IdUser == idUser);
            var word = _context.Words.FirstOrDefault(a => a.Id == idWord);

            if (user == null || word == null)
                return null;

            score = new Score {User = user, Word = word, LastView = GetRepositoryTime()};
            _context.Scores.Add(score);
            _context.SaveChanges();

            return score;
        }

        public IQueryable<WordDatesView> GetWordDates(IQueryable<IWord> words, IQueryable<IScore> scores)
        {
            return words.GroupJoin(scores, w => w.Id, s => s.IdWord, (w, s) => new { w, s })
                .SelectMany(a => a.s.DefaultIfEmpty(),
                    (a, b) =>
                        new WordDatesView
                        {
                            Word = a.w,
                            LastLearned = b != null ? b.LastLearned : null,
                            LastView = b != null ? (DateTime?)b.LastView : null
                        });
        }
        public IQueryable<IScore> GetUserScores(long idUser)
        {
            return _context.Scores.Where(a => a.IdUser == idUser);
        }

        public void EditWord(IWord word)
        {
            var idWord = word.Id;
            var chineseWord = word.OriginalWord;
            var originalWord = _context.Words.FirstOrDefault(a => a.Id == idWord || a.OriginalWord == chineseWord);

            if (originalWord == null)
                throw new Exception(
                    $"Правка невозможна, такого слова в хранилище нет. Id={idWord}, ChineseWord={chineseWord}");

            originalWord.Pronunciation = word.Pronunciation;
            originalWord.Translation = word.Translation;

            _context.SaveChanges();
        }

        public void DeleteWord(long wordId)
        {
            var originalWord = _context.Words.FirstOrDefault(a => a.Id == wordId);

            if (originalWord == null)
                throw new Exception(
                    $"Удаление невозможно, такого слова в хранилище нет. Id={wordId}");

            _context.Words.Remove(originalWord);
            _context.SaveChanges();
        }

        public void AddWord(IWord word)
        {
            var chineseWord = word.OriginalWord;
            var originalWord = _context.Words.FirstOrDefault(a => a.OriginalWord == chineseWord);

            if (originalWord != null)
                throw new Exception($"Слово {chineseWord} уже есть в хранилище.");

            _context.Words.Add(new Word
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
            _context.SaveChanges();
        }
        

        public void AddUser(IUser user)
        {
            var idUser = user.IdUser;
            var originalUser = _context.Users.FirstOrDefault(a => a.IdUser == idUser);

            if (originalUser != null)
                throw new Exception(
                    $"Добавление невозможно, такой пользователь уже существует. idUser={idUser}");

            _context.Users.Add(new User { IdUser = user.IdUser, Name = user.Name });
            _context.SaveChanges();
        }

        public void RemoveUser(long userId)
        {
            var originalUser = _context.Users.FirstOrDefault(a => a.IdUser == userId);

            if (originalUser == null)
                throw new Exception(
                    $"Удаление невозможно, такого пользователя и так не существует. userId={userId}");

            _context.Users.Remove(originalUser);
            _context.SaveChanges();
        }

        public void AddFriendUser(long ownerUserId, long friendUserId)
        {
            var ownerUser = _context.Users.FirstOrDefault(a => a.IdUser == ownerUserId);

            if (ownerUser == null)
                throw new Exception(
                    $"Поделиться списком слов невозможно. ownerUserId={ownerUserId}");


            var friendUser = _context.Users.FirstOrDefault(a => a.IdUser == friendUserId);
            if (friendUser == null)
                throw new Exception(
                    $"Поделиться списком слов невозможно. friendUserId={friendUserId}");

            ownerUser.OwnerUserSharings.Add(new UserSharing { UserFriend = friendUser });
            _context.SaveChanges();
        }

        public void RemoveFriendUser(long ownerUserId, long friendUserId)
        {
            var ownerUser = _context.Users.FirstOrDefault(a => a.IdUser == ownerUserId);

            if (ownerUser == null)
                throw new Exception(
                    $"Забрать доступ к своему словарю невозможно. ownerUserId={ownerUserId}");

            var friendUser = _context.Users.FirstOrDefault(a => a.IdUser == friendUserId);
            if (friendUser == null)
                throw new Exception(
                    $"Забрать доступ к своему словарю невозможно. friendUserId={friendUserId}");

            ownerUser.OwnerUserSharings.Remove(ownerUser.OwnerUserSharings.First(a => a.UserFriend == friendUser));
            _context.SaveChanges();
        }

        public WordStatistic GetCurrentUserWordStatistic(long userId)
        {
            var score =
                _context.Scores.FirstOrDefault(a => a.IdUser == userId && a.IsInLearnMode);

            if (score == null)
                return null;

            return new WordStatistic{ Score = score, Word = score.Word};
        }

        public void SetScore(IScore score)
        {
            var userId = score.IdUser;
            var wordId = score.IdWord;
            var scoreEntity = GetScore(userId, wordId);

            var learnMode = scoreEntity.ToELearnMode();

            scoreEntity.LastLearnMode = score.LastLearnMode;
            scoreEntity.IsInLearnMode = score.IsInLearnMode;
            scoreEntity.OriginalWordCount = score.OriginalWordCount;
            scoreEntity.OriginalWordSuccessCount = score.OriginalWordSuccessCount;
            scoreEntity.PronunciationCount = score.PronunciationCount;
            scoreEntity.PronunciationSuccessCount = score.PronunciationSuccessCount;
            scoreEntity.TranslationCount = score.TranslationCount;
            scoreEntity.TranslationSuccessCount = score.TranslationSuccessCount;
            scoreEntity.ViewCount = score.ViewCount;

            if (learnMode == ELearnMode.FullView)
                scoreEntity.LastView = GetRepositoryTime();
            else
                scoreEntity.LastLearned = GetRepositoryTime();

            _context.SaveChanges();
        }

        #endregion
    }
}
