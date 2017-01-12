using YellowDuck.LearnChinese.Data;
using YellowDuck.LearnChinese.Interfaces;

namespace YellowDuck.LearnChinese.Providers
{
    public abstract class LearnWordProviderBase
    {

        #region Constructors

        protected LearnWordProviderBase(ILearnWordRepository learnRepository)
        {
            LearnRepository = learnRepository;
        }

        #endregion

        #region Fields

        protected readonly ILearnWordRepository LearnRepository;

        #endregion

        #region Methods

        public abstract Poll LearnNextWord(long idUser);

        #endregion
    }
}
