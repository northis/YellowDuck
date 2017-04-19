using System;
using YellowDuck.LearnChinese.Data.ObjectModels;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Extentions;
using YellowDuck.LearnChinese.Interfaces;

namespace YellowDuck.LearnChinese.Providers
{
    public  class ClassicStudyProvider : IStudyProvider
    {

        #region Fields
        
        private readonly IWordRepository _wordRepository;

        public const ushort PollAnswersCount = 4;

        #endregion

        #region Constructors

        public ClassicStudyProvider(IWordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }

        #endregion

        public AnswerResult AnswerWord(long userId, string possibleAnswer)
        {
            var wordStat = _wordRepository.GetCurrentUserWordStatistic(userId);

            if (wordStat == null)
                throw new Exception($"Слов, требующих ответа, для данного пользователя нет. userId={userId}");

            var learnMode = wordStat.Score.ToELearnMode();

            var result = new AnswerResult {Picture = wordStat.Word.CardAll};

            switch (learnMode)
            {
                case ELearnMode.OriginalWord:
                    wordStat.Score.OriginalWordCount++;

                    if (wordStat.Word.OriginalWord == possibleAnswer)
                    {
                        wordStat.Score.OriginalWordSuccessCount++;
                        result.Success = true;
                    }
                    break;

                    case ELearnMode.Pronunciation:
                    wordStat.Score.PronunciationCount++;

                    if (wordStat.Word.Pronunciation == possibleAnswer)
                    {
                        wordStat.Score.PronunciationSuccessCount++;
                        result.Success = true;
                    }
                    break;

                case ELearnMode.Translation:
                    wordStat.Score.TranslationCount++;
                    if (wordStat.Word.Translation == possibleAnswer)
                    {
                        wordStat.Score.TranslationSuccessCount++;
                        result.Success = true;
                    }
                    break;

                case ELearnMode.FullView:
                    result.Success = true;
                    break;
            }

            _wordRepository.SetScore(wordStat.Score);

            return null;
        }

        public LearnUnit LearnWord(long userId, ELearnMode learnMode, EGettingWordsStrategy strategy)
        {
            return
                _wordRepository.GetNextWord(new WordSettings
                {
                    LearnMode = learnMode,
                    Strategy = strategy,
                    UserId = userId,
                    PollAnswersCount = PollAnswersCount
                });
        }
    }
}
