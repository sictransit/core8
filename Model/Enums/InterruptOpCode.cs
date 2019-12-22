namespace Core8.Model.Enums
{
    public enum InterruptOpCode : uint
    {
        SKON = 0,
        ION = 1 << 0,
        IOF = 1 << 1
    }
}
