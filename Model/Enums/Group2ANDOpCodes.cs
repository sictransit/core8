﻿using System;

namespace Core8.Model.Enums
{
    [Flags]
    public enum Group2ANDOpCodes : int
    {
        CLA = 1 << 7,
        SPA = 1 << 6,
        SNA = 1 << 5,
        SZL = 1 << 4,
        SKP = 1 << 3,
    }
}
