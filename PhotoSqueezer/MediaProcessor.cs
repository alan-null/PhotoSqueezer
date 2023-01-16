using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace PhotoSqueezer
{
    public class MediaProcessor
    {
        protected virtual ImageCodecInfo Codec { get; private set; }

        public MediaProcessor()
        {
            Codec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.FormatDescription == "JPEG");
        }

        public virtual Stream Compress(Stream img, long compression = 60L)
        {
            var parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(Encoder.Quality, compression);

            Stream result = new MemoryStream();
            Image.FromStream(img).Save(result, Codec, parameters);
            return result;
        }

        public virtual Stream Resize(Stream img, int width, int height, bool proportionalResize = false, double ratio = -1)
        {
            using (var image = Image.FromStream(img))
            {
                if (proportionalResize)
                {
                    var ratioX = 1.0 * width / image.Width;
                    var ratioY = 1.0 * height / image.Height;
                    ratio = Math.Min(ratioX, ratioY);
                }

                var newWidth = proportionalResize || ratio > 0 ? (int)(image.Width * ratio) : width;
                var newHeight = proportionalResize || ratio > 0 ? (int)(image.Height * ratio) : height;

                var result = new MemoryStream();
                var bitmap = new Bitmap(newWidth, newHeight);
                var graphics = Graphics.FromImage(bitmap);
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                bitmap.Save(result, ImageFormat.Png);
                return result;
            }
        }
    }
}
