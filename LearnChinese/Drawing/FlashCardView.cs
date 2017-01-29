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

        public FlashCardView(SyllableView[] originalWord, SyllableView[] pinyin, string translationNative, string usage, ELearnMode mode)
        {
            OriginalWord = originalWord;
            TranslationNative = translationNative;
            Usage = usage;
            Mode = mode;
            Pinyin = pinyin;
        }

        #endregion

        #region Properties

        public SyllableView[] OriginalWord { get; set; }
        public SyllableView[] Pinyin { get; set; }
        public string TranslationNative { get; set; }
        public string Usage { get; set; }
        public ELearnMode Mode { get; set; }

        public Visibility OriginalWordVisibility => Mode != ELearnMode.OriginalWord
            ? Visibility.Visible
            : Visibility.Collapsed;

        public Visibility PinyinVisibility => Mode != ELearnMode.Pronunciation
            ? Visibility.Visible
            : Visibility.Collapsed;


        public Visibility TranslationVisibility => Mode != ELearnMode.Translation
            ? Visibility.Visible
            : Visibility.Collapsed;

        public Visibility UsageVisibility => Mode == ELearnMode.FullView
            ? Visibility.Visible
            : Visibility.Collapsed;
        
        #endregion

    }
}
