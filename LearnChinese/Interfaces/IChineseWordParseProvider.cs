using YellowDuck.LearnChinese.Data;
using YellowDuck.LearnChinese.Data.ObjectModels;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Interfaces
{
    public interface IChineseWordParseProvider
    {
        Syllable[] GetOrderedSyllables(string word);
        Syllable[] GetOrderedSyllables(IWord word);

        Syllable BuildSyllable(char chineseChar, string pinyinWithNumber);

        ImportWordResult ImportWords(string[] rawWords, bool usePinyin);
    }
}
