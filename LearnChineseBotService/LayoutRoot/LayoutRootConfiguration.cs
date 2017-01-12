using Ninject.Modules;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChinese.Providers;

namespace YellowDuck.LearnChineseBotService.LayoutRoot
{
    public class LayoutRootConfiguration : NinjectModule
    {
        public override void Load()
        {
            Bind<ISyllableColorProvider>().To<ClassicSyllableColorProvider>();
            Bind<IChineseWordParseProvider>().To<PinyinChineseWordParseProvider>();
        }
    }
}
