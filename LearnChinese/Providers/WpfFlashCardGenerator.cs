using System;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YellowDuck.LearnChinese.Data;
using YellowDuck.LearnChinese.Drawing;
using YellowDuck.LearnChinese.Interfaces;
using YellowDuck.LearnChinese.Interfaces.Data;

namespace YellowDuck.LearnChinese.Providers
{
    public class WpfFlashCardGenerator : IFlashCardGenerator
    {
        public byte[] Generate(IWord word)
        {
            var control = new FlashCardTemplate {DataContext = word};


            control.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            control.Arrange(new Rect(control.DesiredSize));

            var res = SaveControlImage(control);
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
            
            var width = (int)control.ActualWidth;
            var height = (int)control.ActualHeight;
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
