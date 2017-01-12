using YellowDuck.LearnChinese.Enums;

namespace YellowDuck.LearnChinese.Interfaces
{
    public interface IChinesePinyinConverter
    {
        string[] Convert(char chineseCharacter, EToneTypes toneType);
        string ToSyllableNumberTone(string syllableMarkTone);
        string[] ToSyllablesAllTones(string syllableMarkTone);
    }
}
