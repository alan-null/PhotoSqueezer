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

        public virtual Stream Compress(Bitmap img, long compression = 50L)
        {
            var parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(Encoder.Quality, compression);

            Stream result = new MemoryStream();
            img.Save(result, Codec, parameters);
            return result;
        }

        public virtual Bitmap Resize(Stream img, int width, int height)
        {
            var image = Image.FromStream(img);

            var ratioX = 1.0 * width / image.Width;
            var ratioY = 1.0 * height / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var bitmap = new Bitmap(newWidth, newHeight);
            var graphics = Graphics.FromImage(bitmap);
            graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            return bitmap;
        }
    }
}
