namespace Core8.Interfaces
{
    public interface IReader
    {
        void Tick();

        void ClearReaderFlag();

        bool IsReaderFlagSet { get; }

        bool IsTapeLoaded { get; }

        uint Buffer { get; }

        void Load(byte[] data);
    }
}
