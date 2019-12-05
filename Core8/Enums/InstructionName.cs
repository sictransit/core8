namespace Core8.Enums
{
    public enum InstructionName : uint
    {
        AND = 0b_000_00_0000000,
        TAD = 0b_001_00_0000000,
        ISZ = 0b_010_00_0000000,
        DCA = 0b_011_00_0000000,
        JMS = 0b_100_00_0000000,
        JMP = 0b_101_00_0000000,
        IOT = 0b_110_000_000_000, // Keyboard, teleprinter ...
        KSF = 0b_110_000_011_001,
        KCC = 0b_110_000_011_010, 
        KRS = 0b_110_000_011_100,        
        KRB = 0b_110_000_011_110,        
        MCI = 0b_111_00_0000000, // Microcoded
    }
}
