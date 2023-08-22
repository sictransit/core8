using Core8.Model.Interfaces;
using System;

namespace Core8.Model;

public abstract class IODevice : IIODevice
{
    public int DeviceId { get; }
    protected int Ticks;

    protected abstract bool InterruptEnable { get; }

    protected virtual int TickDelay => 10;

    public bool InterruptRequested => InterruptEnable && RequestInterrupt;

    protected abstract bool RequestInterrupt { get; }

    protected IODevice(int deviceId)
    {
        if (deviceId is <= 0 or > 0b_111_111) throw new ArgumentOutOfRangeException(nameof(deviceId));

        DeviceId = deviceId;
    }

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