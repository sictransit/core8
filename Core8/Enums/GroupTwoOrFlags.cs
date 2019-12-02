using System;

namespace Core8.Enums
{
    [Flags]
    public enum GroupTwoOrFlags : uint
    {
        CLA = 1 << 7,
        SMA = 1 << 6,
        SZA = 1 << 5,
        SNL = 1 << 4
    }
}
