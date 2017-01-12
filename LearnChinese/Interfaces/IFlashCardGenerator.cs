using YellowDuck.LearnChinese.Data;

namespace YellowDuck.LearnChinese.Interfaces
{
    public interface IFlashCardGenerator
    {
        byte[] Generate(Word word);
    }
}
