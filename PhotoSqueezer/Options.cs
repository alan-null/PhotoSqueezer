using System.Threading.Tasks;

namespace PhotoSqueezer
{
    public class Options
    {
        public ParallelOptions ParallelOptions { get; set; } = new ParallelOptions { MaxDegreeOfParallelism = 5 };
        public int Width { get; set; } = 2000;
        public int Height { get; set; } = 2000;
        public long Compression { get; set; } = 60L;
    }
}