using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YellowDuck.LearnChinese.Drawing;
using YellowDuck.LearnChinese.Enums;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Providers
{
    public class WpfFlashCardGenerator : IFlashCardGenerator
    {
        private readonly IChineseWordParseProvider _wordParseProvider;

        public WpfFlashCardGenerator(IChineseWordParseProvider wordParseProvider)
        {
            _wordParseProvider = wordParseProvider;
        }

        public byte[] Generate(IWord word, ELearnMode learnMode)
        {

            byte[] res = null;
            var tsk = new Thread(() =>
            {
                var isPronunciationMode = learnMode == ELearnMode.Pronunciation;

                var syllables = _wordParseProvider.GetOrderedSyllables(word);
                var wordSyllables =
                    syllables.Select(
                            a => new SyllableView(a.ChineseChar.ToString(), isPronunciationMode ? Colors.Black : a.Color))
                        .ToArray();
                var pinYinSyllables = syllables.Select(a => new SyllableView(a.Pinyin.ToString(), a.Color)).ToArray();

                var view = new FlashCardView(wordSyllables, pinYinSyllables, word.Translation, word.Usage, learnMode);

                var control = new FlashCardTemplate {DataContext = view};

                control.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                control.Arrange(new Rect(control.DesiredSize));
                control.UpdateLayout();

                res = SaveControlImage(control);

            });
            tsk.SetApartmentState(ApartmentState.STA);

            tsk.Start();
            tsk.Join();

            return res;
        }
        

        private byte[] SaveControlImage(FrameworkElement control)
        {
            var rect = VisualTreeHelper.GetDescendantBounds(control);

            var dv = new DrawingVisual();
            
            using (var ctx = dv.RenderOpen())
            {
                var brush = new VisualBrush(control);
                ctx.DrawRectangle(brush, null, new Rect(rect.Size));
            }
            
            var width = (int)control.ActualWidth+1;
            var height = (int)control.ActualHeight+1;
            var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);
            
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);
                return ms.ToArray();
            }
        }
    }

}
