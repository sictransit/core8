namespace Core8.Model.Interfaces
{
    public interface IKeyboard : IIODevice
    {
        bool IsTapeLoaded { get; }

        uint GetBuffer();
    }
}
