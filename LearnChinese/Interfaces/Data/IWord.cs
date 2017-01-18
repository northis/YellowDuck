using System;

namespace YellowDuck.LearnChinese.Interfaces.Data
{
    public interface IWord
    {
        long Id { get; set; }
        string OriginalWord { get; set; }
        string Pronunciation { get; set; }
        DateTime LastModified { get; set; }
        string Translation { get; set; }
        string Usage { get; set; }
        byte[] CardAll { get; set; }
        byte[] CardOriginalWord { get; set; }
        byte[] CardTranslation { get; set; }
        byte[] CardPronunciation { get; set; }
    }
}