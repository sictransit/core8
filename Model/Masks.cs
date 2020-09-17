namespace Core8.Model
{
    public static class Masks
    {
        public const int MEM_WORD
            = 0b_111_111_111_111;

        public const int DF
            = 0b_000_000_000_111;

        public const int IB
            = 0b_000_000_000_111;

        public const int UB
            = 0b_000_000_000_001;

        public const int UF
            = 0b_000_000_000_001;

        public const int SF_DF
            = 0b_000_000_000_111;

        public const int SF_IF
            = 0b_000_000_111_000;

        public const int SF_UF
            = 0b_000_001_000_000;

        public const int OP_CODE
            = 0b_111_000_000_000;

        public const int IOT
            = 0b_110_000_000_000;

        public const int MCI
            = 0b_111_000_000_000;

        public const int IO
            = 0b_000_111_111_000;

        public const int FLOPPY
            = 0b_000_111_000_000;

        public const int IO_OPCODE
            = 0b_000_000_000_111;

        public const int FLOPPY_OPCODE
            = 0b_000_000_000_111;

        public const int FLAG
            = 0b_000_000_000_001;

        public const int IF
            = 0b_111_000_000_000_000;

        public const int ADDRESS_PAGE
            = 0b_111_110_000_000;

        public const int ADDRESS_WORD
            = 0b_000_001_111_111;

        public const int LINK
            = 0b_1_000_000_000_000;

        public const int AC
            = 0b_0_111_111_111_111;

        public const int AC_LINK
            = LINK | AC;

        public const int AC_HIGH
            = 0b_0_111_111_000_000;

        public const int AC_LOW
            = 0b_0_000_000_111_111;

        public const int AC_SIGN
            = 0b_0_100_000_000_000;

        public const int GROUP
            = 0b_000_100_000_000;

        public const int GROUP_1_FLAGS
            = 0b_000_011_111_111;

        public const int GROUP_2_AND
            = 0b_111_100_001_000;

        public const int GROUP_2_AND_OR_FLAGS
            = 0b_000_011_111_000;

        public const int GROUP_3
            = 0b_111_100_000_001;

        public const int GROUP_3_EAE
            = 0b_000_000_101_110;

        public const int GROUP_3_FLAGS
            = 0b_000_011_010_000;

        public const int MEMORY_MANAGEMENT
            = 0b_110_010_000_000;

        public const int INTERRUPT_MASK
            = 0b_000_111_111_000;

        public const int INTERRUPT_FLAGS
            = 0b_000_000_000_111;

        public const int PRIVILEGED_GROUP_2_FLAGS
            = 0b_000_000_000_110;

        public const int TELEPRINTER_BUFFER_MASK
            = 0b_000_001_111_111;

        public const int IO_DEVICE_CONTROL_MASK
            = 0b_000_000_000_011;

        public const int MEM_MGMT_CHANGE_FIELD
            = 0b_000_000_000_011;

        public const int MEM_MGMT_READ
            = 0b_000_000_000_100;
    }
}
