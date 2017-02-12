using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Data
{
    public class AnswerResult
    {
        #region Constructors

        public AnswerResult(IWord rightAnswer)
        {
            Success = rightAnswer == null;
            RightAnswer = rightAnswer;
        }

        public AnswerResult() : this(null)
        {

        }

        #endregion

        #region Methods

        public bool Success { get; private set; }

        public IWord RightAnswer { get; private set; }

        #endregion
    }
}
