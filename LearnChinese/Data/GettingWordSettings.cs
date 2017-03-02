using YellowDuck.LearnChinese.Enums;

namespace YellowDuck.LearnChinese.Data
{

    public class GettingWordSettings
    {
        public long UserId { get; set; }
        public ELearnMode LearnMode { get; set; }
        public EGettingWordsStrategy Strategy { get; set; }
        public ushort PollAnswersCount { get; set; }
    }
}
