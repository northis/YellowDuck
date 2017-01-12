using System.Windows.Media;

namespace YellowDuck.LearnChinese.Interfaces
{
    public interface ISyllableColorProvider
    {
        Color GetSyllableColor(char chineseChar, string pinyinWithNumber);
    }
}