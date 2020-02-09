using CommandLine;

namespace Core8
{
    public class Options
    {
        [Option(Required = false, Default = false, HelpText = "play TINT")]
        public bool TINT { get; set; }

        [Option(Required = false, Default = @"c:\bin\palbart\palbart", HelpText = "location of PALBART (required for asm)")]
        public string PALBART { get; set; }

        [Option(Required = false, HelpText = "assemble and run assembler file")]
        public string Assemble { get; set; }

        [Option(Required = false, Default ="0200", HelpText = "starting address")]
        public string StartingAddress { get; set; }
    }
}
