using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Data.ObjectModels
{
    public class WordMode
    {
        public IWord Word { get; set; }
        public ELearnMode LearnMode { get; set; }
        public GenerateImageResult GenerateImageResult { get; set; }
    }

}
