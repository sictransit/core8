namespace Core8.Enum
{
    public enum InstructionName : int
    {
        AND = 0b_000,
        TAD = 0b_001,
        ISZ = 0b_010,
        DCA = 0b_011,
        JMS = 0b_100,
        JMP = 0b_101,
        Microcoded = 0b_111,
    }
}
