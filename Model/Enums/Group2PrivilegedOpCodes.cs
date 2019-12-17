using System;

namespace Core8.Model.Enums
{
    [Flags]
    public enum Group2PrivilegedOpCodes : uint
    {
        HLT = 1 << 1,
        OSR = 1 << 2,
    }
}
