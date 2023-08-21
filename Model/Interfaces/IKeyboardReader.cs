namespace Core8.Model.Interfaces;

public interface IKeyboardReader : IIODevice
{
    void Type(byte c);

    void Type(byte[] buffer);

    void Clear();

    bool InputFlag { get; }

    void ClearInputFlag();

    byte InputBuffer { get; }

    void SetDeviceControl(int data);

    void MountPaperTape(byte[] chars);

    void RemovePaperTape();
}