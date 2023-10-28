using System.Threading.Tasks;

namespace PhotoSqueezer
{
    public class Options
    {
        public ParallelOptions ParallelOptions { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public long Compression { get; set; }
        public double Ratio { get; set; }
        public bool Verify { get; set; }
        public bool ProportionalResize { get; set; }
    }
}