using YellowDuck.LearnChinese.Data;
using YellowDuck.LearnChinese.Enums;

namespace YellowDuck.LearnChinese.Interfaces
{
    public interface IStudyProvider
    {
        string[] LearnWord(long userId, ELearnMode learnMode, EGettingWordsStrategy strategy);

        AnswerResult AnswerWord(long userId, string possibleAnswer);

    }
}
