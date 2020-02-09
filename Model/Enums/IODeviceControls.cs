using System;

namespace Core8.Model.Enums
{
    [Flags]
    public enum IODeviceControls
    {
        InterruptEnable = 1 << 0,
        StatusEnable = 1 << 1,
    }
}
