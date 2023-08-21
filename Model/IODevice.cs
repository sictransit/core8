using Core8.Model.Interfaces;

namespace Core8.Model;

public abstract class IODevice : IIODevice
{
    protected int Ticks;

    public abstract bool InterruptRequested { get; }

    protected abstract bool InterruptEnable { get; }

    protected virtual int TickDelay => 10;

    public void Tick()
    {
        if (Ticks++ > TickDelay)
        {
            Ticks = 0;

            HandleTick();
        }
    }

    protected abstract void HandleTick();
}