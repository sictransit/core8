namespace Core8
{
    public class Masks
    {
        public const uint INSTRUCTION = 0b_1111_1111_1111;
        
        public const uint ADDRESSING_MODE = 0b_000_11_0000000;

        public const uint ADDRESS_WORD = 0b_111_1111;

        public const uint MEM_WORD = 0b_1111_1111_1111;

        public const uint OP_CODE = 0b_111_00_0000000;



        public const uint FLAG = 0b_1;

        public const uint IF = 0b_111_00000_0000000;
        public const uint PAGE = 0b_11111_0000000;
        public const uint WORD = 0b_00000_1111111;

        public const uint LINK = 0b_1_0000_0000_0000;
        public const uint AC = 0b_0_111_111_111_111;
        public const uint AC_HIGH = 0b_0_111_111_000_000;
        public const uint AC_LOW = 0b_0_000_000_111_111;
        public const uint AC_SIGN = 0b_0_1000_0000_0000;

        public const uint GROUP = 0b_0001_0000_0000;
        public const uint GROUP_2 = 0b_1111_0000_0000;
        public const uint GROUP_2_PRIV = 0b_110;
        

        public const uint GROUP_1 = 0b_1110_1111_1111;
        public const uint GROUP_1_FLAGS = 0b_0000_1111_1111;
        public const uint GROUP_2_AND = 0b_0000_1000;
        public const uint GROUP_2_AND_FLAGS = 0b_1111_1000;
        public const uint GROUP_2_OR_FLAGS = 0b_1111_0000;
        public const uint PRIVILEGED_GROUP_2_FLAGS = 0b_110;

    
    }
}
