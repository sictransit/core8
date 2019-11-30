using System;
using System.Collections.Generic;
using System.Text;

namespace Core8
{
    public class Masks
    {
        public const uint INSTRUCTION = 0b_1111_1111_1111;
        public const uint I_MODE = 0b_000_10_0000000;
        public const uint Z_MODE = 0b_000_01_0000000;
        public const uint ADDRESS_WORD = 0b_111_1111;

        public const uint MEM_WORD = 0b_1111_1111_1111;

        public const uint OP_CODE = 0b_111_00_0000000;

        

        public const uint FLAG = 0b_1;

        public const uint IF = 0b_111_00000_0000000;
        public const uint PAGE = 0b_11111_0000000;
        public const uint WORD = 0b_00000_1111111;

        public const uint LINK = 0b_1_0000_0000_0000;
        public const uint AC = 0b_0_1111_1111_1111;
        public const uint AC_HIGH = 0b_0_1111_1100_0000;
        public const uint AC_LOW = 0b_0_0000_0011_1111;

        public const uint GROUP = 0b_0001_0000_0000;
        public const uint GROUP_2 = 0b_1111_0000_0000;
        public const uint GROUP_2_AND_OR = 0b_1111_0000_1000;
        public const uint GROUP_2_OSR = 0b_1111_0000_0100;
        public const uint GROUP_2_HLT = 0b_1111_0000_0010;

        public const uint GROUP_1 = 0b_1111_1111;

        public const uint GROUP_1_CLA = 0b_1000_0000;
        public const uint GROUP_1_CLL = 0b_0100_0000;
        public const uint GROUP_1_CMA = 0b_0010_0000;
        public const uint GROUP_1_CML = 0b_0001_0000;
        public const uint GROUP_1_IAC = 0b_0000_0001;
        public const uint GROUP_1_RAR = 0b_0000_1000;
        public const uint GROUP_1_RAL = 0b_0000_0100;
        public const uint GROUP_1_RTR = 0b_0000_1010;
        public const uint GROUP_1_RTL = 0b_0000_0110;
        public const uint GROUP_1_BSW = 0b_0000_0010;
    }
}
