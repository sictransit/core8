namespace Core8.Model.Interfaces
{
    public interface IKeyboard : IIODevice, IInputDevice
    {
        bool IsTapeLoaded { get; }

        uint Buffer { get; }
    }
}
