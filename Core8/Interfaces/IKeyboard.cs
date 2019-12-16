namespace Core8.Interfaces
{
    public interface IKeyboard : IIODevice
    {
        bool IsTapeLoaded { get; }

        uint Buffer { get; }

        void Load(byte[] data);
    }
}
