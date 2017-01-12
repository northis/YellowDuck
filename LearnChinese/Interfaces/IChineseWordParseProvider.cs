using YellowDuck.LearnChinese.Data;

namespace YellowDuck.LearnChinese.Interfaces
{
    public interface IChineseWordParseProvider
    {
        Syllable[] GetOrderedSyllables(string word);
        Syllable[] GetOrderedSyllables(Word word);

        Syllable BuildSyllable(char chineseChar, string pinyinWithNumber);

        ImportWordResult ImportWords(string[] rawWords, bool usePinyin);
    }
}
