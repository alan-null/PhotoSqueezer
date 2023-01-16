using CommandLine;

namespace PhotoSqueezer
{
    public class InputOptions
    {
        [Option('s', "source", Required = true, HelpText = "Source folder")]
        public string Source { get; set; } = string.Empty;

        [Option('d', "destination", Required = true, HelpText = "Destination folder")]
        public string Destination { get; set; } = string.Empty;

        [Option('w', "width", Required = false, HelpText = "Image width", Default = 2000, Group = "Size")]
        public int Width { get; set; }

        [Option('h', "height", Required = false, HelpText = "Image height", Default = 2000, Group = "Size")]
        public int Height { get; set; }

        [Option('c', "compression", Required = false, HelpText = "Image compression", Default = 60L)]
        public long Compression { get; set; }

        [Option('t', "threads", Required = false, HelpText = "Max degree of parallelism", Default = 5)]
        public int Threads { get; set; }

        [Option('p', "proportionalResize", Required = false, HelpText = "Resize proportionaly", Default = false, Group = "Size")]
        public bool ProportionalResize { get; set; }

        [Option('r', "ratio", Required = false, HelpText = "Aspect ratio for resize", Default = 0, Group = "Ratio")]
        public double Ratio { get; set; }
    }
}