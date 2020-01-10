namespace Core8.Model.Interfaces
{
    public interface IIODevice
    {
        void Tick();

        bool IsFlagSet { get; }

        void ClearFlag();

        void Clear();

        void Type(byte c);

        void Type(byte[] chars);
    }
}
