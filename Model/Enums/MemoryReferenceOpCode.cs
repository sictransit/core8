namespace Core8.Model.Enums
{
    public enum MemoryReferenceOpCode : int
    {
        AND = 0b_000_00_0000000,
        TAD = 0b_001_00_0000000,
        ISZ = 0b_010_00_0000000,
        DCA = 0b_011_00_0000000,
        JMS = 0b_100_00_0000000,
        JMP = 0b_101_00_0000000,
    }
}
