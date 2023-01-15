using System.IO;

namespace PhotoSqueezer
{
    class Program
    {
        static void Main(string[] args)
        {
            string src = @"C:\dir-in";
            string dst = @"C:\dir-out";

            if (!Directory.Exists(src))
            {
                System.Console.WriteLine("Incorrect source directory");
                return;

            }
            if (!Directory.Exists(dst))
            {
                System.Console.WriteLine("Incorrect destination directory");
                return;
            }

            var s = new Squeezer();
            s.Sqeeze(src, dst);
        }
    }
}
