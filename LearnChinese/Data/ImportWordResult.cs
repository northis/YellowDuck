using YellowDuck.LearnChinese.Data.Ef;

namespace YellowDuck.LearnChinese.Data
{
    public class ImportWordResult
    {
        #region Constructors

        public ImportWordResult(Word[] successfulWords, string[] failedWords)
        {
            SuccessfulWords = successfulWords;
            FailedWords = failedWords;
        }

        #endregion

        #region Methods

        public Word[] SuccessfulWords { get; }
        public string[] FailedWords { get; }

        #endregion

    }
}
