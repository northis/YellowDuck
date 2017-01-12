using System;
using System.Linq.Expressions;
using YellowDuck.LearnChinese.Data;

namespace YellowDuck.LearnChinese.Interfaces
{
    public interface ILearnWordRepository
    {
        Word[] GetWords(Expression<Func<Word, bool>> whereCondition);
        

        DateTime GetRepositoryTime();
        
        void EditWord(Word word);

        void DeleteWord(long wordId);

        void AddWord(Word word);

        Score GetScore(long idUser, long idWord);

        void SetScore(Score score);


        void AddUser(User user);
        void RemoveUser(long userId);
    }
}
