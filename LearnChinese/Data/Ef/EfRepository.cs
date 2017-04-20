using System;
using System.Linq;
using System.Linq.Expressions;
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
        
        IOrderedQueryable<Score> GetDifficultScores(ELearnMode learnMode, IQueryable<Score> scores)
        {
            switch (learnMode)
            {
                case ELearnMode.OriginalWord:
                    return
                        scores.OrderByDescending(
                            a =>
                                a.OriginalWordCount > 0 && a.OriginalWordCount > 0
                                    ? a.OriginalWordSuccessCount / a.OriginalWordCount
                                    : 0);

                case ELearnMode.Pronunciation:
                    return
                        scores.OrderByDescending(
                            a =>
                                a.PronunciationCount > 0 && a.PronunciationCount > 0
                                    ? a.PronunciationSuccessCount / a.PronunciationCount
                                    : 0);

                case ELearnMode.Translation:
                    return
                        scores.OrderByDescending(
                            a =>
                                a.TranslationCount > 0 && a.TranslationCount > 0
                                    ? a.TranslationSuccessCount / a.TranslationCount
                                    : 0);
            }

            return scores.OrderBy(a => 0);
        }

        void SetUnscoredWords(long idUser)
        {

            var unscoredUserWordIds =
                _context.Words.Where(
                        a =>
                            !a.Scores.Any() &&
                            (a.IdOwner == idUser || a.UserOwner.OwnerUserSharings.Any(b => b.IdFriend == idUser)))
                    .Select(a => a.Id);

            foreach (var idWord in unscoredUserWordIds)
            {
                GetScore(idUser, idWord);
            }
        }

        public LearnUnit GetNextWord(WordSettings settings)
        {
            var userId = settings.UserId;
            var learnMode = settings.LearnMode;
            var strategy = settings.Strategy;
            var pollAnswersCount = settings.PollAnswersCount;

            SetUnscoredWords(userId);

            var scores =
                _context.Scores.Where(a => a.IdUser == userId).OrderBy(a => 0);

            var difficultScores = GetDifficultScores(learnMode, scores);

            IQueryable<IWord> userWords;
            switch (strategy)
            {
                case EGettingWordsStrategy.NewFirst:

                    userWords = scores.ThenByDescending(a => a.LastLearned ?? DateTime.MaxValue)
                        .ThenByDescending(a => a.LastView)
                        .ThenByDescending(a => a.Word.LastModified)
                        .Select(a => a.Word);
                    break;

                case EGettingWordsStrategy.NewMostDifficult:

                    userWords = difficultScores.ThenByDescending(a => a.LastLearned ?? DateTime.MaxValue)
                        .ThenByDescending(a => a.LastView)
                        .ThenByDescending(a => a.Word.LastModified)
                        .Select(a => a.Word);
                    break;

                case EGettingWordsStrategy.OldFirst:

                    userWords = scores.ThenBy(a => a.LastLearned ?? DateTime.MaxValue)
                        .ThenBy(a => a.LastView)
                        .ThenBy(a => a.Word.LastModified)
                        .Select(a => a.Word);
                    break;

                case EGettingWordsStrategy.OldMostDifficult:

                    userWords = difficultScores.ThenBy(a => a.LastLearned ?? DateTime.MaxValue)
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

            var answers = userWords.Take(pollAnswersCount).OrderBy(a => Guid.NewGuid());
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

        /*
        public IOrderedQueryable<WordDatesView> GetWordDates(IQueryable<IWord> words, IOrderedQueryable<IScore> scores)
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
        }*/

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

        public void AddWord(IWord word, long idUser)
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
                Id = word.Id,
                IdOwner = idUser
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

        public WordStatistic GetUserWordStatistic(long userId, long wordId)
        {
            var score =
                _context.Scores.FirstOrDefault(a => a.IdUser == userId && a.IdWord == wordId);

            if (score == null)
            {
                var userOwners = _context.UserSharings.Where(a => a.IdFriend == userId).Select(a=>a.IdOwner).Distinct();
                var word =
                    _context.Words.FirstOrDefault(
                        a => a.Id == wordId && (a.IdOwner == userId || userOwners.Contains(a.IdOwner)));

                if(word == null)
                    return null;

                return new WordStatistic {Score = null, Word = word};
            }

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

        public bool IsUserExist(long userId)
        {
            return _context.Users.Any(a => a.IdUser == userId);
        }

        public void SetUserCommand(long userId, string command)
        {
            var user = _context.Users.FirstOrDefault(a => a.IdUser == userId);

            if (user == null)
                throw new Exception($"Пользователь с Id={userId} не найден");

            user.LastCommand = command;
            _context.SaveChanges();
        }

        public string GetUserCommand(long userId)
        {
            var user = _context.Users.FirstOrDefault(a => a.IdUser == userId);

            if (user == null)
                throw new Exception($"Пользователь с Id={userId} не найден");

            return user.LastCommand;
        }

        public IWord GetWord(string wordOriginal)
        {
            var word = _context.Words.FirstOrDefault(a => a.OriginalWord == wordOriginal);

            if (word == null)
                throw new Exception($"Слова/фразы '{wordOriginal}' нет в хранилище");

            return word;
        }

        #endregion
    }
}
