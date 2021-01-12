using CommandLine;

namespace rpsls.Console
{    
    public sealed class Options
    {
        [Option('t', "training", Required = false, HelpText = "Genenates a ML training set.")]
        public bool Training { get; set; }
    }
}
