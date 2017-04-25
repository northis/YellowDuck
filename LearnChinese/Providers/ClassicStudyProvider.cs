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
                    wordStat.Score.OriginalWordCount = (wordStat.Score.OriginalWordCount ?? 0) + 1;

                    if (wordStat.Word.OriginalWord == possibleAnswer)
                    {
                        wordStat.Score.OriginalWordSuccessCount = (wordStat.Score.OriginalWordSuccessCount ?? 0) + 1;
                        result.Success = true;
                    }
                    break;

                    case ELearnMode.Pronunciation:
                    wordStat.Score.PronunciationCount += (wordStat.Score.PronunciationCount ?? 0) + 1;

                    if (wordStat.Word.Pronunciation == possibleAnswer)
                    {
                        wordStat.Score.PronunciationSuccessCount = (wordStat.Score.PronunciationSuccessCount ?? 0) + 1;
                        result.Success = true;
                    }
                    break;

                case ELearnMode.Translation:
                    wordStat.Score.TranslationCount += (wordStat.Score.TranslationCount ?? 0) + 1;
                    if (wordStat.Word.Translation == possibleAnswer)
                    {
                        wordStat.Score.TranslationSuccessCount = (wordStat.Score.TranslationSuccessCount ?? 0) + 1;
                        result.Success = true;
                    }
                    break;

                case ELearnMode.FullView:
                    result.Success = true;
                    break;
            }

            _wordRepository.SetScore(wordStat.Score);

            return result;
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
