using System;
using System.Linq;
using YellowDuck.LearnChinese.Data;
using YellowDuck.LearnChinese.Data.Ef;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Providers
{
    public  class ClassicStudyProvider : IStudyProvider
    {

        #region Fields

        private readonly Func<LearnChineseDbContext> _getContext;
        private readonly IWordRepository _wordRepository;

        public const ushort PollAnswersCount = 4;

        #endregion

        #region Constructors

        public ClassicStudyProvider(Func<LearnChineseDbContext> getContext, IWordRepository wordRepository)
        {
            _getContext = getContext;
            _wordRepository = wordRepository;
        }

        #endregion


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
                    return null;

                score = new Score { User = user, Word = word, LastView = _wordRepository.GetRepositoryTime() };
                cntxt.Scores.Add(score);
                cntxt.SaveChanges();

                return score;
            }
        }

        public string[] LearnWord(long userId, ELearnMode learnMode, EGettingWordsStrategy strategy)
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
                score.LastLearned = _wordRepository.GetRepositoryTime();

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

                return stringAnswers;
            }
        }


        private IQueryable<Score> GetDifficultScores(ELearnMode learnMode, IQueryable<Score> scores)
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

        private IQueryable<WordDatesView> GetWordDates(LearnChineseDbContext cntxt, IQueryable<Score> scores)
        {
            return cntxt.Words.GroupJoin(scores, w => w.Id, s => s.IdWord, (w, s) => new { w, s })
                .SelectMany(a => a.s.DefaultIfEmpty(),
                    (a, b) =>
                        new WordDatesView
                        {
                            Word = a.w,
                            LastLearned = b != null ? b.LastLearned : null,
                            LastView = b != null ? (DateTime?)b.LastView : null
                        });
        }



        public AnswerResult AnswerWord(long userId, string possibleAnswer)
        {
            using (var cntxt = _getContext())
            {
                var userScore = cntxt.Scores.FirstOrDefault(a => a.IdUser == userId);

            }

            return null;
        }
    }
}
