namespace Core8.Peripherals.Floppy.Declarations
{
    public static class Functions
    {
        public const int FILL_BUFFER = 0 << 1;
        public const int EMPTY_BUFFER = 1 << 1;
        public const int WRITE_SECTOR = 2 << 1;
        public const int READ_SECTOR = 3 << 1;
        public const int NO_OPERATION = 4 << 1;
        public const int READ_STATUS = 5 << 1;
        public const int WRITE_DELETED_DATA_SECTOR = 6 << 1;
        public const int READ_ERROR_REGISTER = 7 << 1;
    }
}
