using YellowDuck.LearnChinese.Data.ObjectModels;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Interfaces
{
    public interface IChineseWordParseProvider
    {
        Syllable BuildSyllable(char chineseChar, string pinyinWithNumber);
        Syllable[] GetOrderedSyllables(string word);
        Syllable[] GetOrderedSyllables(IWord word);

        ImportWordResult ImportWords(string[] rawWords, bool usePinyin);
    }
}