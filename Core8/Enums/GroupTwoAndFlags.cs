using System;

namespace Core8.Enums
{
    [Flags]
    public enum GroupTwoAndFlags : uint
    {
        CLA = 1 << 7,
        SPA = 1 << 6,
        SNA = 1 << 5,
        SZL = 1 << 4
    }
}
