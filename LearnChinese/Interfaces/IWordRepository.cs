using System;
using System.Linq;
using YellowDuck.LearnChinese.Data.ObjectModels;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Interfaces
{
    public interface IWordRepository
    {
        WordStatistic GetCurrentUserWordStatistic(long userId);
        WordStatistic GetUserWordStatistic(long userId, long wordId);

        LearnUnit GetNextWord(WordSettings settings);

        DateTime GetRepositoryTime();

        void SetScore(IScore score);
        
        void EditWord(IWord word);

        void DeleteWord(long wordId);

        void AddWord(IWord word, long idUser);
        
        void AddUser(IUser user);

        IQueryable<IUser> GetUsers();
        IQueryable<IUser> GetUserFriends(long userId);

        IWord GetWord(string wordOriginal);
        IQueryable<IWord> GetTopWords(string searchPattern, long userId);

        bool IsUserExist(long userId);

        void SetUserCommand(long userId, string command);
        string GetUserCommand(long userId);

        void AddFriendUser(long ownerUserId, long friendUserId);

        void RemoveFriendUser(long ownerUserId, long friendUserId);

        void RemoveUser(long userId);
    }
}
