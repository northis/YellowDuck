using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Data
{
    public sealed class Poll
    {
        #region Constructors

        public Poll(string[] answers, IWord wordToCheck)
        {
            Answers = answers;
            WordToCheck = wordToCheck;
        }

        #endregion

        #region Properties

        public string[] Answers { get;  }
        public IWord WordToCheck { get;  }

        #endregion
    }
}
