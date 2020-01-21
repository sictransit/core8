using System;

namespace Core8.Model.Enums
{    
    public enum MemoryManagementReadOpCode : uint
    {
        RDF = 0b_110_010_001_100,
        RIF = 0b_110_010_010_100,
        RIB = 0b_110_010_011_100,
        RMF = 0b_110_010_100_100,
    }
}
