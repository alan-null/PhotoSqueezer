using CommandLine;
using System.Threading.Tasks;

namespace PhotoSqueezer
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<InputOptions>(args).WithParsed(o => new Squeezer(Parse(o)).Sqeeze(o.Source, o.Destination));
        }

        private static Options Parse(InputOptions o)
        {
            return new Options
            {
                Width = o.Width,
                Height = o.Height,
                Compression = o.Compression,
                ParallelOptions = new ParallelOptions { MaxDegreeOfParallelism = o.Threads }
            };
        }
    }
}
