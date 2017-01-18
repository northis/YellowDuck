using System;
using System.Linq.Expressions;
using YellowDuck.LearnChinese.Data;
using YellowDuck.LearnChinese.Data.Ef;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Interfaces
{
    public interface ILearnWordRepository
    {
        IWord[] GetWords(Expression<Func<IWord, bool>> whereCondition);
        

        DateTime GetRepositoryTime();
        
        void EditWord(IWord word);

        void DeleteWord(long wordId);

        void AddWord(IWord word);

        Poll LearnWord(long userId, ELearnMode learnMode, EGettingWordsStrategy strategy);

        void SetScore(Score score);


        void AddUser(IUser user);
        void RemoveUser(long userId);
    }
}
