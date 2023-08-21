namespace Core8.Model.Interfaces;

public interface IIODevice
{
    bool InterruptRequested { get; }

    void Tick();
}
