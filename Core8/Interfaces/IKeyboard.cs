namespace Core8.Interfaces
{
    public interface IKeyboard : IIODevice
    {
        void ClearFlag();

        bool IsFlagSet { get; }

        bool IsTapeLoaded { get; }

        uint Buffer { get; }

        void Load(byte[] data);
    }
}
