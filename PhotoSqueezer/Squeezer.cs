﻿using ImageMagick;
using System;
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
        protected MediaProcessor mediaProcessor { get; private set; }
        protected Options Options { get; }

        public Squeezer() : this(new Options()) { }

        public Squeezer(Options o)
        {
            mediaProcessor = new MediaProcessor();
            Options = o;
        }

        public void Sqeeze(string src, string dst)
        {
            Console.WriteLine("Squeezing");
            Console.WriteLine($"From: {src}");
            Console.WriteLine($"To: {dst}");
            Console.WriteLine(string.Empty);
            Console.WriteLine($"Parameters");
            Console.WriteLine($"Width: {Options.Width}");
            Console.WriteLine($"Height: {Options.Height}");
            Console.WriteLine($"Compression: {Options.Compression}");
            Console.WriteLine($"MaxDegreeOfParallelism: {Options.ParallelOptions.MaxDegreeOfParallelism}");

            CreateFolderStructure(src, dst);

            var missing = GetMissingFiles(src, dst).Select(path => new FileInfo(path));

            Parallel.ForEach(missing, Options.ParallelOptions, fi => { TransformFile(fi, fi.FullName.Replace(src, dst)); });
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
                    if (Options.Verify)
                    {
                        Console.WriteLine($"CreateDirectory({d})");
                    }
                    else
                    {
                        Directory.CreateDirectory(d);
                    }
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
                var jpgStream = new MemoryStream();
                using (var file = File.OpenRead(obj.FullName))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        using (var image = new MagickImage(memoryStream.ToArray()))
                        {
                            image.Write(jpgStream, MagickFormat.Jpg);
                        }
                    }
                }
                return jpgStream;
            }
            else
            {
                return File.OpenRead(obj.FullName);
            }
        }

        protected virtual void TransformFile(FileInfo source, string path)
        {
            if (Options.Verify)
            {
                Console.WriteLine($"{source.FullName}=>{path}");
                return;
            }
            using (var file = GetFile(source))
            {
                using (var resized = mediaProcessor.Resize(file, Options.Width, Options.Height, Options.ProportionalResize, Options.Ratio))
                {
                    using (var compressed = mediaProcessor.Compress(resized, Options.Compression))
                    {
                        Image image = CopyMeta(file, compressed);
                        using (var result = File.Create(path))
                        {
                            image.Save(result, ImageFormat.Jpeg);
                        }
                        image.Dispose();
                    }
                }
            }
        }

        protected virtual Image CopyMeta(Stream from, Stream to)
        {
            var src = Image.FromStream(from);
            {
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
}