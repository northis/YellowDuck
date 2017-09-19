using System.Windows.Media;

namespace YellowDuck.LearnChinese.Drawing
{
    public sealed class SyllableView
    {
        #region Constructors

        public SyllableView()
        {
        }

        public SyllableView(string text, Color color)
        {
            Text = text;
            Brush = new SolidColorBrush(color);
        }

        #endregion

        #region Methods

        public string Text { get; set; }

        public SolidColorBrush Brush { get; set; }

        #endregion
    }
}