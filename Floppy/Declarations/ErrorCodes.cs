namespace Core8.Peripherals.Floppy.Declarations
{
    public static class ErrorCodes
    {
        public const int BAD_TRACK_ADDRESS = 0b_000_000_100_000; // 0040

        public const int SEEK_FAILED = 0b_000_000_111_000; // 0070
    }
}
