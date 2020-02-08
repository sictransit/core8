namespace Core8.Model.Enums
{
    public enum InterruptOpCode : uint
    {
        SKON = 0b_000,
        ION = 0b_001,
        IOF = 0b_010,
        SRQ = 0b_011,
        GTF = 0b_100,
        RTF = 0b_101,
        SGT = 0b_110,
        CAF = 0b_111,
    }
}
