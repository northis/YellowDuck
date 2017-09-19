using YellowDuck.LearnChinese.Data.ObjectModels;
using YellowDuck.LearnChinese.Enums;

namespace YellowDuck.LearnChinese.Interfaces
{
    public interface IStudyProvider
    {
        AnswerResult AnswerWord(long userId, string possibleAnswer);
        LearnUnit LearnWord(long userId, ELearnMode learnMode);
    }
}