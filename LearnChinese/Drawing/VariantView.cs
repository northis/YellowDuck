using System.Windows;

namespace YellowDuck.LearnChinese.Drawing
{
    public class VariantView
    {
        #region Constructors

        public VariantView()
        {
            ChineseFontVisibility = Visibility.Collapsed;
        }

        public VariantView(ushort orderNumber, string text, bool useChineseFont)
        {
            OrderNumber = orderNumber;
            Text = text;
            ChineseFontVisibility = useChineseFont ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region Properties

        public ushort OrderNumber { get; set; }
        public string Text { get; set; }
        public Visibility ChineseFontVisibility { get; set; }

        public Visibility CommonFontVisibility => ChineseFontVisibility == Visibility.Visible
            ? Visibility.Collapsed
            : Visibility.Visible;

        #endregion
    }
}