using System;
using YellowDuck.LearnChinese.Data;
using YellowDuck.LearnChinese.Data.ObjectModels;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Interfaces
{
    public interface IWordRepository
    {
        WordStatistic GetCurrentUserWordStatistic(long userId);

        LearnUnit GetNextWord(WordSettings settings);

        DateTime GetRepositoryTime();

        void SetScore(IScore score);
        
        void EditWord(IWord word);

        void DeleteWord(long wordId);

        void AddWord(IWord word);
        
        void AddUser(IUser user);

        bool IsUserExist(long userId);

        void SetUserCommand(long userId, string command);
        string GetUserCommand(long userId);

        void AddFriendUser(long ownerUserId, long friendUserId);

        void RemoveFriendUser(long ownerUserId, long friendUserId);

        void RemoveUser(long userId);
    }
}
