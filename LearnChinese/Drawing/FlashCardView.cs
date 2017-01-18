using System.Windows;
using YellowDuck.LearnChinese.Enums;

namespace YellowDuck.LearnChinese.Drawing
{
    public class FlashCardView
    {
        #region Constructors


        public FlashCardView()
        {
            //Mode = ELearnModes.All;
        }

        public FlashCardView(SyllableView[] originalWord, SyllableView[] pinyin, string translationNative, int successLearnCount, int totalLearnCount, string usage, EViewMode mode, VariantView[] variants)
        {
            OriginalWord = originalWord;
            TranslationNative = translationNative;
            TotalLearnCount = totalLearnCount;
            Usage = usage;
            Mode = mode;
            Variants = variants;
            SuccessLearnCount = successLearnCount;
            Pinyin = pinyin;
        }

        #endregion

        #region Properties

        public SyllableView[] OriginalWord { get; set; }
        public SyllableView[] Pinyin { get; set; }
        public VariantView[] Variants { get; set; }
        public string TranslationNative { get; set; }
        public int SuccessLearnCount { get; set; }
        public int TotalLearnCount { get; set; }
        public string Usage { get; set; }
        public EViewMode Mode { get; set; }

        public Visibility OriginalWordVisibility => Mode == EViewMode.All || Mode == EViewMode.OriginalWordOnly ||
                                                Mode == EViewMode.OriginalWordAndPronunciation
            ? Visibility.Visible
            : Visibility.Collapsed;

        public Visibility PinyinVisibility => Mode == EViewMode.All || Mode == EViewMode.PronunciationOnly ||
                                                Mode == EViewMode.PronunciationAndTranslation || Mode == EViewMode.OriginalWordAndPronunciation
            ? Visibility.Visible
            : Visibility.Collapsed;


        public Visibility TranslationVisibility => Mode == EViewMode.All || Mode == EViewMode.TranslationOnly || Mode == EViewMode.PronunciationAndTranslation
            ? Visibility.Visible
            : Visibility.Collapsed;

        public Visibility StatisticVisibility => Mode == EViewMode.All ? Visibility.Visible : Visibility.Collapsed;

        #endregion

    }
}
