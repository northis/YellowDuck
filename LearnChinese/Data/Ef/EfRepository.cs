using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        private readonly bool _useFullText;
        public const int MaxSearchResults = 5;
        #endregion

        #region Constructors

        public EfRepository(LearnChineseDbContext context, bool useFullText)
        {
            _context = context;
            _useFullText = useFullText;
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
                        scores.OrderBy(
                            a =>
                                a.OriginalWordCount > 0 && a.OriginalWordCount > 0
                                    ? a.OriginalWordSuccessCount / a.OriginalWordCount
                                    : 0);

                case ELearnMode.Pronunciation:
                    return
                        scores.OrderBy(
                            a =>
                                a.PronunciationCount > 0 && a.PronunciationCount > 0
                                    ? a.PronunciationSuccessCount / a.PronunciationCount
                                    : 0);

                case ELearnMode.Translation:
                    return
                        scores.OrderBy(
                            a =>
                                a.TranslationCount > 0 && a.TranslationCount > 0
                                    ? a.TranslationSuccessCount / a.TranslationCount
                                    : 0);
            }

            return scores.OrderBy(a => a.ViewCount);
        }

        void SetUnscoredWords(long idUser)
        {

            var unscoredUserWordIds =
                _context.Words.Where(
                        a =>
                            a.Scores.All(b => b.IdUser != idUser) &&
                            (a.IdOwner == idUser || a.UserOwner.OwnerUserSharings.Any(b => b.IdFriend == idUser)))
                    .Select(a => a.Id);

            foreach (var idWord in unscoredUserWordIds.ToArray())
            {
                GetScore(idUser, idWord);
            }
        }

        public void SetLearnMode(long userId, EGettingWordsStrategy mode)
        {
            var user = _context.Users.FirstOrDefault(a => a.IdUser == userId);

            if (user == null)
                throw new Exception(
                    $"User doesn't exist. userId={userId}");

            user.Mode = mode.ToString();

            _context.SaveChanges();
        }

        public EGettingWordsStrategy GetLearnMode(long userId)
        {
            var user = _context.Users.FirstOrDefault(a => a.IdUser == userId);

            if (user == null)
                throw new Exception(
                    $"User doesn't exist. userId={userId}");

            if (!Enum.TryParse(user.Mode, true, out EGettingWordsStrategy strategy))
                SetLearnMode(userId, EGettingWordsStrategy.Random);

            return strategy;
        }

        public LearnUnit GetNextWord(WordSettings settings)
        {
            var userId = settings.UserId;
            var learnMode = settings.LearnMode;
            var strategy = GetLearnMode(userId);
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
                        .Select(a => a.Word);
                    break;

                case EGettingWordsStrategy.OldFirst:

                    userWords = scores.ThenBy(a => a.LastLearned ?? DateTime.MinValue)
                        .ThenBy(a => a.LastView)
                        .ThenBy(a => a.Word.LastModified)
                        .Select(a => a.Word);
                    break;

                case EGettingWordsStrategy.OldMostDifficult:

                    userWords = difficultScores.ThenBy(a => a.LastLearned ?? DateTime.MinValue)
                        .ThenBy(a => a.LastView)
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
                throw new Exception($"No suitable words to learn. userId={userId}");

            var wordId = word.Id;
            var score = GetScore(userId, wordId);

            score.LastLearnMode = learnMode.ToString();
            score.IsInLearnMode = learnMode != ELearnMode.FullView;
            score.LastLearned = GetRepositoryTime();

            var answers =
                userWords.Where(a => a.SyllablesCount == word.SyllablesCount)
                    .Take(pollAnswersCount)
                    .OrderBy(a => Guid.NewGuid());
            var questionItem = new LearnUnit();

            switch (learnMode)
            {
                case ELearnMode.OriginalWord:
                    questionItem.Options = answers.Select(a => a.OriginalWord).ToArray();
                    questionItem.Picture = word.CardOriginalWord;
                    break;

                case ELearnMode.Pronunciation:
                    questionItem.Options = answers.Select(a => a.Pronunciation).ToArray();
                    questionItem.Picture = word.CardPronunciation;
                    break;

                case ELearnMode.Translation:
                    questionItem.Options = answers.Select(a => a.Translation).ToArray();
                    questionItem.Picture = word.CardTranslation;
                    break;

                case ELearnMode.FullView:
                    if (score.ViewCount == null)
                        score.ViewCount = 0;

                    score.ViewCount++;
                    questionItem.Options = new string[0];
                    questionItem.Picture = word.CardAll;
                    questionItem.WordStatistic = GetUserWordStatistic(userId, wordId).ToString();
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

            score = new Score
            {
                User = user,
                Word = word,
                LastView = GetRepositoryTime(),
                LastLearnMode = ELearnMode.FullView.ToString(),
                IsInLearnMode = false
            };
            _context.Scores.Add(score);
            _context.SaveChanges();

            return score;
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
                    $"Editing is not possible, there is no such word in the dictionary. Id={idWord}, ChineseWord={chineseWord}");

            originalWord.Pronunciation = word.Pronunciation;
            originalWord.Translation = word.Translation;
            originalWord.OriginalWord = word.OriginalWord;
            originalWord.LastModified = GetRepositoryTime();
            originalWord.Translation = word.Translation;
            originalWord.Usage = word.Usage;

            if (word.CardAll != null)
                originalWord.CardAll = word.CardAll;

            if (word.CardOriginalWord != null)
                originalWord.CardOriginalWord = word.CardOriginalWord;

            if (word.CardPronunciation != null)
                originalWord.CardPronunciation = word.CardPronunciation;

            if (word.CardTranslation != null)
                originalWord.CardTranslation = word.CardTranslation;

            _context.SaveChanges();
        }

        public void DeleteWord(long wordId)
        {
            var originalWord = _context.Words.FirstOrDefault(a => a.Id == wordId);

            if (originalWord == null)
                throw new Exception(
                    $"Removing is not possible, there is no such word in the dictionary. Id={wordId}");

            
            _context.Scores.RemoveRange(_context.Scores.Where(a => a.IdWord == wordId));
            _context.Words.Remove(originalWord);
            _context.SaveChanges();
        }

        public void AddWord(IWord word, long idUser)
        {
            var chineseWord = word.OriginalWord;
            var originalWord = _context.Words.FirstOrDefault(a => a.OriginalWord == chineseWord);

            if (originalWord != null)
                throw new Exception($"Слово {chineseWord} уже есть в хранилище.");


            var wrdNew = new Word
            {
                LastModified = GetRepositoryTime(),
                OriginalWord = word.OriginalWord,
                Pronunciation = word.Pronunciation,
                Translation = word.Translation,
                Usage = word.Usage,
                Id = word.Id,
                IdOwner = idUser,
                SyllablesCount = word.SyllablesCount,

                CardAll = word.CardAll,
                CardOriginalWord = word.CardOriginalWord,
                CardTranslation = word.CardTranslation,
                CardPronunciation = word.CardPronunciation
            };
            

            _context.Words.Add(wrdNew);
            _context.SaveChanges();
        }
        

        public void AddUser(IUser user)
        {
            var idUser = user.IdUser;
            var originalUser = _context.Users.FirstOrDefault(a => a.IdUser == idUser);

            if (originalUser != null)
                throw new Exception(
                    $"Can't add this user, because he already exists. idUser={idUser}");

            _context.Users.Add(new User { IdUser = user.IdUser, Name = user.Name, JoinDate = GetRepositoryTime() });
            _context.SaveChanges();
        }

        public void RemoveUser(long userId)
        {
            var originalUser = _context.Users.FirstOrDefault(a => a.IdUser == userId);

            if (originalUser == null)
                throw new Exception(
                    $"Can't delete this user, there is no such user. userId={userId}");

            _context.Users.Remove(originalUser);
            _context.SaveChanges();
        }

        public void AddFriendUser(long ownerUserId, long friendUserId)
        {
            var ownerUser = _context.Users.FirstOrDefault(a => a.IdUser == ownerUserId);

            if (ownerUser == null)
                throw new Exception(
                    $"Can't share current word list. ownerUserId={ownerUserId}");


            var friendUser = _context.Users.FirstOrDefault(a => a.IdUser == friendUserId);
            if (friendUser == null)
                throw new Exception(
                    $"Can't share current word list. friendUserId={friendUserId}");

            ownerUser.OwnerUserSharings.Add(new UserSharing { UserFriend = friendUser });
            _context.SaveChanges();
        }

        public void RemoveFriendUser(long ownerUserId, long friendUserId)
        {
            var ownerUser = _context.Users.FirstOrDefault(a => a.IdUser == ownerUserId);

            if (ownerUser == null)
                throw new Exception(
                    $"Can't remove user from the share list. ownerUserId={ownerUserId}");

            var friendUser = _context.Users.FirstOrDefault(a => a.IdUser == friendUserId);
            if (friendUser == null)
                throw new Exception(
                    $"Can't remove user from the share list. friendUserId={friendUserId}");

            _context.UserSharings.RemoveRange(
                ownerUser.OwnerUserSharings.Where(a => a.IdOwner == ownerUserId && a.IdFriend == friendUserId));
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

                score = new Score
                {
                    IdUser = userId,
                    IdWord = wordId,
                    LastView = GetRepositoryTime(),
                    ViewCount = 1,
                    LastLearnMode = ELearnMode.FullView.ToString()
                };
                
            }
            else
            {
                score.LastView = GetRepositoryTime();
                score.ViewCount += 1;
            }

            SetScore(score);
            return new WordStatistic{ Score = score, Word = score.Word};
        }

        public void SetScore(IScore score)
        {
            var userId = score.IdUser;
            var wordId = score.IdWord;
            var scoreEntity = GetScore(userId, wordId);

            var learnMode =  score.ToELearnMode();

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

            if (score.IsInLearnMode)
            {
                foreach (var scores in _context.Scores.Where(a => a.IdUser == userId))
                    scores.IsInLearnMode = false;
            }

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
                throw new Exception($"User with Id={userId} is not found");

            user.LastCommand = command;
            _context.SaveChanges();
        }

        public string GetUserCommand(long userId)
        {
            var user = _context.Users.FirstOrDefault(a => a.IdUser == userId);

            if (user == null)
                throw new Exception($"User with Id={userId} is not found");

            return user.LastCommand;
        }

        public IWord GetWord(string wordOriginal)
        {
            var word = _context.Words.FirstOrDefault(a => a.OriginalWord == wordOriginal);

            if (word == null)
                throw new Exception($"No such word as '{wordOriginal}' in the dictionary");

            return word;
        }

        public IQueryable<IUser> GetUsers()
        {
            return _context.Users;
        }
        public IQueryable<IUser> GetUserFriends(long userId)
        {
            return _context.UserSharings.Where(a => a.IdOwner == userId).Select(a => a.UserFriend);
        }

        public IQueryable<WordSearchResult> FindFlashCard(string searchString, long userId)
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return new WordSearchResult[] {}.AsQueryable();

            if (_useFullText)
            {
                return
                    _context.Database.SqlQuery<WordSearchResult>(
                            $"SELECT top ({MaxSearchResults}) f.IdWord as FileId, f.Height as HeightFlashCard, f.Width as WidthFlashCard, w.OriginalWord, w.Pronunciation, w.Translation   FROM [LearnChinese].[dbo].[Word] w join [LearnChinese].[dbo].[WordFileA] f on (f.IdWord = w.Id and w.IdOwner={userId})  where  CONTAINS(w.OriginalWord, '{searchString}')")
                        .AsQueryable();
            }

            return _context.Words.Where(a => a.IdOwner == userId && a.OriginalWord.Contains(searchString))
                .Take(MaxSearchResults)
                .Select(
                    a =>
                        new WordSearchResult
                        {
                            FileId = a.WordFileA.IdWord,
                            OriginalWord = a.OriginalWord,
                            Pronunciation = a.Pronunciation,
                            Translation = a.Translation,
                            HeightFlashCard = a.WordFileA.Height,
                            WidthFlashCard = a.WordFileA.Width
                        });
        }

        public byte[] GetWordFlashCard(long fileId)
        {
            return _context.WordFileAs.Where(a => a.IdWord == fileId).Select(a => a.Bytes).FirstOrDefault();
        }

        #endregion
    }
}
