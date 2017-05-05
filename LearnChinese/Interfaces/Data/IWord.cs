using System;
using YellowDuck.LearnChinese.Data.Ef;
using YellowDuck.LearnChinese.Data.ObjectModels;

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
        GenerateImageResult CardAll { get; set; }
        GenerateImageResult CardOriginalWord { get; set; }
        GenerateImageResult CardTranslation { get; set; }
        GenerateImageResult CardPronunciation { get; set; }

        int SyllablesCount { get; set; }
    }
}