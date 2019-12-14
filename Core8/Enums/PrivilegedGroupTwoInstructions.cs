using System;

namespace Core8.Enums
{
    [Flags]
    public enum PrivilegedGroupTwoInstructions : uint
    {
        HLT = 1 << 1,
        OSR = 1 << 2,
    }
}
