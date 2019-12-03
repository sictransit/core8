using System;

namespace Core8.Enums
{
    [Flags]
    public enum GroupTwoAndFlags : uint
    {
        CLA = 1 << 7 | 1 << 3,
        SPA = 1 << 6 | 1 << 3,
        SNA = 1 << 5 | 1 << 3,
        SZL = 1 << 4 | 1 << 3
    }
}
