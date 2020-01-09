using System;

namespace Core8.Model.Enums
{
    [Flags]
    public enum Group2OROpCodes : uint
    {
        CLA = 1 << 7,
        SMA = 1 << 6,
        SZA = 1 << 5,
        SNL = 1 << 4,
    }
}
