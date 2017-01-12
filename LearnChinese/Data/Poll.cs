namespace YellowDuck.LearnChinese.Data
{
    public sealed class Poll
    {
        #region Constructors

        public Poll(Word[] answers, Word wordToCheck)
        {
            Answers = answers;
            WordToCheck = wordToCheck;
        }

        #endregion

        #region Properties

        public Word[] Answers { get; }
        public Word WordToCheck { get; }

        #endregion
    }
}
