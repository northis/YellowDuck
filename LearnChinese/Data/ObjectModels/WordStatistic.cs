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
                return "0";

            return
                $"写{Score.OriginalWordSuccessCount??0}({Score.OriginalWordCount??0}), 听{Score.PronunciationSuccessCount??0}({Score.PronunciationCount??0}), 翻译{Score.TranslationSuccessCount??0}({Score.TranslationCount??0}), 看{Score.ViewCount}";
        }
    }
}
