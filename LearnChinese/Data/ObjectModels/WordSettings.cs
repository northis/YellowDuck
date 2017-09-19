using YellowDuck.LearnChinese.Enums;

namespace YellowDuck.LearnChinese.Data.ObjectModels
{
    public class WordSettings
    {
        public long UserId { get; set; }
        public ELearnMode LearnMode { get; set; }
        public ushort PollAnswersCount { get; set; }
    }
}