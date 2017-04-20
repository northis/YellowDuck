using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Data.ObjectModels
{
    public class WordStatistic
    {
        public IWord Word { get; set; }
        public IScore Score { get; set; }

        public override string ToString()
        {
            if (Score == null)
                return base.ToString();

            return Score.ViewCount.ToString();//TODO
        }
    }
}
