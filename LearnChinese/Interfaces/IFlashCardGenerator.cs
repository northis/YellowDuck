using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Interfaces
{
    public interface IFlashCardGenerator
    {
        byte[] Generate(IWord word, ELearnMode learnMode);
    }
}
