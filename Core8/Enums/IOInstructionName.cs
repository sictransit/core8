using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Enums
{
    public enum IOInstructionName : uint
    {
        KCF = 0b_110_000_011_000,
        KSF = 0b_110_000_011_001,
        KCC = 0b_110_000_011_010,
        KRS = 0b_110_000_011_100,
        KRB = 0b_110_000_011_110,
        TSF = 0b_110_000_100_001,
        TLS = 0b_110_000_100_110,
        TPC = 0b_110_010_001_100,
    }
}
