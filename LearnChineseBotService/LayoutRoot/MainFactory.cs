using Ninject;

namespace YellowDuck.LearnChineseBotService.LayoutRoot
{
    public static class MainFactory
    {
        #region Constructors

        static MainFactory()
        {
            Init();
        }

        #endregion

        #region Properties

        public static StandardKernel NinjectKernel { get; private set; }

        #endregion

        #region Methods

        public static void Init()
        {
            if (NinjectKernel == null)
                NinjectKernel = new StandardKernel(new LayoutRootConfiguration());

        }

        #endregion
    }
}
