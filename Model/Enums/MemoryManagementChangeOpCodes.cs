using System;

namespace Core8.Model.Enums
{
    [Flags]
    public enum MemoryManagementChangeOpCodes : int
    {
        CDF = 1 << 0,
        CIF = 1 << 1
    }
}
