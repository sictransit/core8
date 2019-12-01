using System;

namespace Core8.Enum
{
    public enum InstructionName : int
    {
        AND = 0b_000_00_0000000,
        TAD = 0b_001_00_0000000,
        ISZ = 0b_010_00_0000000,
        DCA = 0b_011_00_0000000,
        JMS = 0b_100_00_0000000,
        JMP = 0b_101_00_0000000,
        Microcoded = 0b_111_00_0000000,
    }

    [Flags]
    public enum AddressingModes : uint
    {
        Z = 1 << 7,
        I = 1 << 8
    }

    [Flags]
    public enum GroupOneFlags : uint
    {
        IAC = 1 << 0,
        BSW = 1 << 1,
        RAL = 1 << 2,
        RAR = 1 << 3,
        CML = 1 << 4,
        CMA = 1 << 5,
        CLL = 1 << 6,
        CLA = 1 << 7,
    }

    [Flags]
    public enum GroupTwoOrFlags : uint
    {
        CLA = 1 << 7,
        SMA = 1 << 6,
        SZA = 1 << 5,
        SNL = 1 << 4
    }

    [Flags]
    public enum GroupTwoAndFlags : uint
    {
        CLA = 1 << 7,
        SPA = 1 << 6,
        SNA = 1 << 5,
        SZL = 1 << 4
    }

    [Flags]
    public enum PrivilegedGroupTwoFlags : uint
    {
        HLT = 1<<1,
        OSR = 1<<2,
    }
}
