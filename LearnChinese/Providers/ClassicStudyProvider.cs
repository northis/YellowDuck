using YellowDuck.LearnChinese.Data;
using YellowDuck.LearnChinese.Enums;
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
           // _wordRepository.GetUserScores(userId);

            return null;
        }

        public string[] LearnWord(long userId, ELearnMode learnMode, EGettingWordsStrategy strategy)
        {
            return
                _wordRepository.GetNexWord(new GettingWordSettings
                {
                    LearnMode = learnMode,
                    Strategy = strategy,
                    UserId = userId,
                    PollAnswersCount = PollAnswersCount
                });
        }
    }
}
