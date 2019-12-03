namespace Core8.Interfaces
{
    public interface IReader
    {
        void ClearReaderFlag();

        bool IsReaderFlagSet { get; }

        uint Buffer { get; }
    }
}
