using System.Collections.Generic;

namespace YellowDuck.LearnChinese.Interfaces
{
    public interface ISyllablesToStringConverter
    {
        IEnumerable<string> Parse(string pinyinString);

        string Join(IEnumerable<string> syllables);

        string GetSeparator();

    }
}
