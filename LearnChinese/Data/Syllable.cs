using System.Windows.Media;

namespace YellowDuck.LearnChinese.Data
{
    public  class Syllable
    {
        #region Constructors

        public Syllable(char chineseChar, string pinyin, Color color)
        {
            ChineseChar = chineseChar;
            Pinyin = pinyin;
            Color = color;
        }

        #endregion

        #region Properties

        public char ChineseChar { get; set; }
        public string Pinyin { get; set; }
        public Color Color { get; set; }

        #endregion
    }
}
