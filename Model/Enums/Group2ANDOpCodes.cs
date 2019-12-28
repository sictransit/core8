﻿using System;

namespace Core8.Model.Enums
{
    [Flags]
    public enum Group2ANDOpCodes : uint
    {
        SKP = 0,
        CLA = 1 << 7,
        SPA = 1 << 6,
        SNA = 1 << 5,
        SZL = 1 << 4
    }
}
