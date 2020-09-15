namespace Core8.Floppy.Declarations
{
    public class ErrorCodes
    {
        public const int BadTrackAddress = 0b_000_000_100_000; // 0040

        public const int SeekFailed = 0b_000_000_111_000; // 0070
    }
}
