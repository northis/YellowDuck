namespace YellowDuck.LearnChinese.Data.ObjectModels
{
    public class AnswerResult
    {
        #region Methods

        public bool Success { get; set; }

        public string RightAnswer { get; set; }

        public byte[] Picture { get; set; }

        #endregion
    }
}
