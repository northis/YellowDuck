using System;
using System.Linq.Expressions;
using YellowDuck.LearnChinese.Data;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Interfaces
{
    public interface IWordRepository
    {
        IWord[] GetWords(Expression<Func<IWord, bool>> whereCondition);

        string[] GetNexWord(GettingWordSettings settings);

        DateTime GetRepositoryTime();
        
        void EditWord(IWord word);

        void DeleteWord(long wordId);

        void AddWord(IWord word);
        
        void AddUser(IUser user);

        void AddFriendUser(long ownerUserId, long friendUserId);

        void RemoveFriendUser(long ownerUserId, long friendUserId);

        void RemoveUser(long userId);
    }
}
