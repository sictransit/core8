using CommandLine;

namespace Core8
{
    public class Options
    {
        [Option(Required = false, Default = false, HelpText = "play TINT")]
        public bool TINT { get; set; }

        [Option(Required = false, Default = @"c:\bin\palbart\palbart.exe", HelpText = "PALBART executable, required for assemble")]
        public string PALBART { get; set; }

        [Option(Required = false, HelpText = "PAL assembly file")]
        public string Assemble { get; set; }

        [Option(Required = false, HelpText = "load bin format paper tape")]
        public string Load { get; set; }

        [Option(Required = false, Default = false, HelpText = "run the assembled file")]
        public bool Run { get; set; }

        [Option(Required = false, Default = false, HelpText = "dump tty output to console")]
        public bool TTY { get; set; }

        [Option(Required = false, Default = 200, HelpText = "starting address")]
        public int StartingAddress { get; set; }

        [Option(Required = false, Default = false, HelpText = "dump memory")]
        public bool DumpMemory { get; set; }

        [Option(Required = false, Default = false, HelpText = "floppy development and debugging")]
        public bool Floppy { get; set; }

        [Option(Required = false, Default = false, HelpText = "debug mode")]
        public bool Debug { get; set; }

        [Option(Required = false, HelpText = "convert ASCII string to octal words")]
        public string Convert { get; set; }

        [Option(Required = false, HelpText = "punch paper tape, i.e. copy bin image")]
        public string Punch { get; set; }

    }
}
