namespace YellowDuck.LearnChinese.Data
{
    public struct UserWord
    {
        public UserWord(long idUser, long idWord)
        {
            IdUser = idUser;
            IdWord = idWord;
        }

        public long IdUser { get; }

        public long IdWord { get; }
    }
}
