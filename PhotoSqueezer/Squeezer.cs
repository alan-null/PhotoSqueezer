using ImageMagick;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoSqueezer
{
    public class Squeezer
    {
        public static MediaProcessor mediaProcessor { get; private set; }

        public Squeezer()
        {
            mediaProcessor = new MediaProcessor();
        }

        public void Sqeeze(string src, string dst)
        {
            CreateFolderStructure(src, dst);

            var missing = GetMissingFiles(src, dst).Select(path => new FileInfo(path));

            var options = new ParallelOptions { MaxDegreeOfParallelism = 5 };
            Parallel.ForEach(missing, options, fi => { TransformFile(fi, fi.FullName.Replace(src, dst)); });
        }

        protected virtual void CreateFolderStructure(string startPath, string destination)
        {
            string[] folders = Directory.GetDirectories(startPath, "*", SearchOption.AllDirectories);
            var relativePaths = folders.Select(f => f.Remove(0, startPath.Length));

            var desitnations = relativePaths.Select(r => $"{destination}\\{r}".Replace("\\\\", "\\"));
            foreach (var d in desitnations)
            {
                if (!Directory.Exists(d))
                {
                    Directory.CreateDirectory(d);
                }
            }
        }

        protected virtual IEnumerable<string> GetMissingFiles(string startPath, string destination)
        {
            const SearchOption options = SearchOption.AllDirectories;
            var files = Directory.GetFiles(startPath, "*.jpeg", options)
                    .Union(Directory.GetFiles(startPath, "*.heic", options))
                    .Union(Directory.GetFiles(startPath, "*.jpg", options));
            return files.Where(f => !File.Exists(f.Replace(startPath, destination)));
        }

        protected virtual Stream GetFile(FileInfo obj)
        {
            if (obj.Extension.Equals(".heic"))
            {
                using (var file = File.OpenRead(obj.FullName))
                {
                    var jpgStream = new MemoryStream();
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        using (var image = new MagickImage(memoryStream.ToArray()))
                        {
                            image.Write(jpgStream, MagickFormat.Jpg);
                        }
                    }
                    return jpgStream;
                }
            }
            else
            {
                return File.OpenRead(obj.FullName);
            }
        }

        protected virtual void TransformFile(FileInfo source, string path)
        {
            using (var file = GetFile(source))
            {
                var resized = mediaProcessor.Resize(file, 2000, 2000);
                using (var compressed = mediaProcessor.Compress(resized, 60))
                {
                    Image image = CopyMeta(file, compressed);
                    using (var result = File.Create(path))
                    {
                        image.Save(result, ImageFormat.Jpeg);
                    }
                }
            }
        }

        protected virtual Image CopyMeta(Stream from, Stream to)
        {
            var src = Image.FromStream(from);
            var dst = Image.FromStream(to);

            foreach (var p in dst.PropertyIdList)
            {
                dst.RemovePropertyItem(p);
            }

            foreach (var p in src.PropertyItems)
            {
                dst.SetPropertyItem(p);
            }
            return dst;
        }
    }
}
