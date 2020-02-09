using CommandLine;

namespace Core8
{
    public class Options
    {
        [Option(Required = false, Default = false, HelpText = "play TINT")]
        public bool TINT { get; set; }

        [Option(Required = false, Default = @"c:\bin\palbart\palbart.exe", HelpText = "PALBART executable, required for assemble")]
        public string PALBART { get; set; }

        [Option(Required = false, HelpText = "assemble file")]
        public string Assemble { get; set; }

        [Option(Required = false, Default = false, HelpText = "run the assembled file")]
        public bool Run { get; set; }

        [Option(Required = false, Default = 200u, HelpText = "starting address")]
        public uint StartingAddress { get; set; }
    }
}
