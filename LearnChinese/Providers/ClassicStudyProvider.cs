﻿using System;
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
                throw new Exception($"There no  words for this user which must be answered. userId={userId}");

            var learnMode = wordStat.Score.ToELearnMode();

            var result = new AnswerResult {WordStatistic = wordStat};

            switch (learnMode)
            {
                case ELearnMode.OriginalWord:
                    if (wordStat.Score.OriginalWordCount == null)
                        wordStat.Score.OriginalWordCount = 0;
                    if (wordStat.Score.OriginalWordSuccessCount == null)
                        wordStat.Score.OriginalWordSuccessCount = 0;

                    wordStat.Score.OriginalWordCount = wordStat.Score.OriginalWordCount + 1;

                    if (wordStat.Word.OriginalWord == possibleAnswer)
                    {
                        wordStat.Score.OriginalWordSuccessCount = wordStat.Score.OriginalWordSuccessCount + 1;
                        result.Success = true;
                    }
                    break;

                    case ELearnMode.Pronunciation:
                    if (wordStat.Score.PronunciationCount == null)
                        wordStat.Score.PronunciationCount = 0;
                    if (wordStat.Score.PronunciationSuccessCount == null)
                        wordStat.Score.PronunciationSuccessCount = 0;

                    wordStat.Score.PronunciationCount += wordStat.Score.PronunciationCount + 1;

                    if (wordStat.Word.Pronunciation == possibleAnswer)
                    {
                        wordStat.Score.PronunciationSuccessCount = wordStat.Score.PronunciationSuccessCount + 1;
                        result.Success = true;
                    }
                    break;

                case ELearnMode.Translation:
                    if (wordStat.Score.TranslationCount == null)
                        wordStat.Score.TranslationCount = 0;
                    if (wordStat.Score.TranslationSuccessCount == null)
                        wordStat.Score.TranslationSuccessCount = 0;

                    wordStat.Score.TranslationCount += wordStat.Score.TranslationCount + 1;
                    if (wordStat.Word.Translation == possibleAnswer)
                    {
                        wordStat.Score.TranslationSuccessCount = wordStat.Score.TranslationSuccessCount + 1;
                        result.Success = true;
                    }
                    break;

                case ELearnMode.FullView:
                    result.Success = true;
                    break;
            }

            wordStat.Score.IsInLearnMode = false;
            _wordRepository.SetScore(wordStat.Score);

            return result;
        }

        public LearnUnit LearnWord(long userId, ELearnMode learnMode)
        {
            return
                _wordRepository.GetNextWord(new WordSettings
                {
                    LearnMode = learnMode,
                    UserId = userId,
                    PollAnswersCount = PollAnswersCount
                });
        }
    }
}
