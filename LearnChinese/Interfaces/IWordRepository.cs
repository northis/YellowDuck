using System;
using System.Linq;
using YellowDuck.LearnChinese.Data.ObjectModels;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Interfaces
{
    public interface IWordRepository
    {
        WordStatistic GetCurrentUserWordStatistic(long userId);
        WordStatistic GetUserWordStatistic(long userId, long wordId);

        IQueryable<WordSearchResultItem> FindFlashCard(string searchString, long userId);

        DateTime GetRepositoryTime();

        void SetScore(IScore score);
        
        void EditWord(IWord word);

        void DeleteWord(long wordId);

        void AddWord(IWord word, long idUser);
        
        void AddUser(IUser user);

        IQueryable<IUser> GetUsers();
        IQueryable<IUser> GetUserFriends(long userId);

        IWord GetWord(string wordOriginal);

        bool IsUserExist(long userId);

        void SetUserCommand(long userId, string command);
        string GetUserCommand(long userId);

        void AddFriendUser(long ownerUserId, long friendUserId);

        void RemoveFriendUser(long ownerUserId, long friendUserId);

        void RemoveUser(long userId);

        void SetLearnMode(long userId, EGettingWordsStrategy mode);

        EGettingWordsStrategy GetLearnMode(long userId);

        LearnUnit GetNextWord(WordSettings settings);

        byte[] GetWordFlashCard(string fileId);

    }
}
