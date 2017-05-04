using YellowDuck.LearnChinese.Data.ObjectModels;
using YellowDuck.LearnChinese.Enums;

namespace YellowDuck.LearnChinese.Interfaces
{
    public interface IStudyProvider
    {
        LearnUnit LearnWord(long userId, ELearnMode learnMode);

        AnswerResult AnswerWord(long userId, string possibleAnswer);

    }
}
