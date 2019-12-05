namespace Core8.Interfaces
{
    public interface IKeyboard
    {
        void Tick();

        void ClearFlag();

        bool IsFlagSet { get; }

        bool IsTapeLoaded { get; }

        uint Buffer { get; }

        void Load(byte[] data);
    }
}
