using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AIMLTGBot
{
    class Processor
    {
        
        static int border = 20;
        static int top = 40;
        static int left = 40;
        static int Width = 500;
        static int Height = 500;
        static float differenceLim = 0.15f;

        public static Bitmap ProcessImage(Bitmap bitmap)
        {
            // На вход поступает необработанное изображение с веб-камеры

            AForge.Imaging.Filters.Invert InvertFilter = new AForge.Imaging.Filters.Invert();

            int side = bitmap.Height;

            //  Отпиливаем границы, но не более половины изображения
            if (side < 4 * border) border = side / 4;
            side -= 2 * border;

            //  Мы сейчас занимаемся тем, что красиво оформляем входной кадр, чтобы вывести его на форму
            Rectangle cropRect = new Rectangle((bitmap.Width - bitmap.Height) / 2 + left + border, top + border, side, side);

            //  Тут создаём новый битмапчик, который будет исходным изображением
            var original = new Bitmap(cropRect.Width, cropRect.Height);

            //  Объект для рисования создаём
            Graphics g = Graphics.FromImage(original);

            g.DrawImage(bitmap, new Rectangle(0, 0, original.Width, original.Height), cropRect, GraphicsUnit.Pixel);
            Pen p = new Pen(Color.Red);
            p.Width = 1;

            //  Теперь всю эту муть пилим в обработанное изображение
            AForge.Imaging.Filters.Grayscale grayFilter = new AForge.Imaging.Filters.Grayscale(0.2125, 0.7154, 0.0721);
            var uProcessed = grayFilter.Apply(AForge.Imaging.UnmanagedImage.FromManagedImage(original));

            //  Масштабируем изображение до 500x500 - этого достаточно
            InvertFilter.ApplyInPlace(uProcessed);
            AForge.Imaging.Filters.ResizeBilinear scaleFilter = new AForge.Imaging.Filters.ResizeBilinear(200, 200);
            AForge.Imaging.Filters.Crop cropfilter = new AForge.Imaging.Filters.Crop(new Rectangle(75, 75, 200, 200));
            // apply the filter
            InvertFilter.ApplyInPlace(uProcessed);

            uProcessed = scaleFilter.Apply(uProcessed);
            original = scaleFilter.Apply(original);
            //uProcessed = cropfilter.Apply(uProcessed);
            g = Graphics.FromImage(original);
            //  Пороговый фильтр применяем. Величина порога берётся из настроек, и меняется на форме
            AForge.Imaging.Filters.BradleyLocalThresholding threshldFilter = new AForge.Imaging.Filters.BradleyLocalThresholding();
            threshldFilter.PixelBrightnessDifferenceLimit = differenceLim;
            threshldFilter.ApplyInPlace(uProcessed);

            return uProcessed.ToManagedImage();
        }

        static public string GetNameFigure()
        {
            string[] stypes = new string[] { "Play", "Pause", "Stop", "Volume", "Next", "Prev" };
            Random rand = new Random();
            int r = rand.Next(0, 5);
            return stypes[r];
        }
    }
}
