namespace Core8.Interfaces
{
    public interface IIODevice
    {
        void Tick();

        uint Id { get; }

        bool IsFlagSet { get; }

        void ClearFlag();

        void Clear();
    }
}
