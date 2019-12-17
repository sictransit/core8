using System;

namespace Core8.Model.Enums
{
    [Flags]
    public enum AddressingModes : uint
    {
        Z = 1 << 7,
        I = 1 << 8
    }
}
