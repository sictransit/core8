﻿namespace Core8.Model
{
    public static class Masks
    {
        public const uint BRANCHING
            = 0b_100_00_0000000;

        public const uint ADDRESSING_MODE
            = 0b_000_110_000_000;

        public const uint MEM_WORD
            = 0b_111_111_111_111;

        public const uint DF
            = 0b_000_000_000_111;

        public const uint IB
            = 0b_000_000_000_111;

        public const uint UB
            = 0b_000_000_000_001;

        public const uint UF
            = 0b_000_000_000_001;

        public const uint SF_DF
            = 0b_000_000_000_111;

        public const uint SF_IF
            = 0b_000_000_111_000;

        public const uint SF_UF
            = 0b_000_001_000_000;

        public const uint OP_CODE
            = 0b_111_000_000_000;

        public const uint IOT
            = 0b_110_000_000_000;

        public const uint MCI
            = 0b_111_000_000_000;

        public const uint IO
            = 0b_000_111_111_000;

        public const uint IO_OPCODE
            = 0b_000_000_000_111;

        public const uint FLAG
            = 0b_000_000_000_001;

        public const uint IF
            = 0b_111_000_000_000_000;

        public const uint ADDRESS_PAGE
            = 0b_111_110_000_000;

        public const uint ADDRESS_WORD
            = 0b_000_001_111_111;

        public const uint LINK
            = 0b_1_000_000_000_000;

        public const uint AC
            = 0b_0_111_111_111_111;

        public const uint AC_LINK
            = LINK | AC;

        public const uint AC_HIGH
            = 0b_0_111_111_000_000;

        public const uint AC_LOW
            = 0b_0_000_000_111_111;

        public const uint AC_SIGN
            = 0b_0_100_000_000_000;

        public const uint GROUP
            = 0b_000_100_000_000;

        public const uint GROUP_1_FLAGS
            = 0b_000_011_111_111;

        public const uint GROUP_2_AND
            = 0b_111_100_001_000;

        public const uint GROUP_2_AND_OR_FLAGS
            = 0b_000_011_111_000;

        public const uint GROUP_3
            = 0b_111_100_000_001;

        public const uint GROUP_3_EAE
            = 0b_000_000_101_110;

        public const uint GROUP_3_FLAGS
            = 0b_000_011_010_000;

        public const uint MEMORY_MANAGEMENT
            = 0b_110_010_000_000;

        public const uint INTERRUPT_MASK
            = 0b_000_111_111_000;

        public const uint INTERRUPT_FLAGS
            = 0b_000_000_000_111;

        public const uint PRIVILEGED_GROUP_2_FLAGS
            = 0b_000_000_000_110;

        public const uint KEYBOARD_BUFFER_MASK
            = 0b_000_011_111_111;

        public const uint TELEPRINTER_BUFFER_MASK
            = 0b_000_001_111_111;

        public const uint IO_DEVICE_CONTROL_MASK
            = 0b_000_000_000_011;

        public const uint MEM_MGMT_CHANGE_FIELD
            = 0b_000_000_000_011;

        public const uint MEM_MGMT_READ
            = 0b_000_000_000_100;
    }
}
