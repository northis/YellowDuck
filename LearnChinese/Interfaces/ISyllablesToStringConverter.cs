using System.Collections.Generic;

namespace YellowDuck.LearnChinese.Interfaces
{
    public interface ISyllablesToStringConverter
    {
        string GetSeparator();

        string Join(IEnumerable<string> syllables);
        IEnumerable<string> Parse(string pinyinString);
    }
}