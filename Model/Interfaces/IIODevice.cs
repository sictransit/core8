namespace Core8.Model.Interfaces;

public interface IIODevice
{
    bool InterruptRequested { get; }

    int DeviceId { get; }

    void Tick();
}
