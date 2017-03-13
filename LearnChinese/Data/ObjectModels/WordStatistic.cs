using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Data.ObjectModels
{
    public class WordStatistic
    {
        public IWord Word { get; set; }
        public IScore Score { get; set; }
    }
}
