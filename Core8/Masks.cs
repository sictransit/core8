using System;
using System.Collections.Generic;
using System.Text;

namespace Core8
{
    public class Masks
    {
        public const ushort INSTRUCTION = 0b_1111_1111_1111;
        public const ushort I_MODE = 0b_000_10_0000000;
        public const ushort Z_MODE = 0b_000_01_0000000;
        public const ushort ADDRESS_WORD = 0b_111_1111;

        public const ushort MEM_WORD = 0b_1111_1111_1111;

        public const ushort OP_CODE = 0b_111_00_0000000;

        

        public const ushort FLAG = 0b_1;

        public const ushort IF = 0b_111_00000_0000000;
        public const ushort PAGE = 0b_11111_0000000;
        public const ushort WORD = 0b_00000_1111111;

        public const ushort LINK = 0b_1_0000_0000_0000;
        public const ushort AC = 0b_0_1111_1111_1111;
    }
}
