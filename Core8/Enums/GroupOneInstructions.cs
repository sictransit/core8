using System;

namespace Core8.Enums
{
    [Flags]
    public enum GroupOneInstructions : uint
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
}
