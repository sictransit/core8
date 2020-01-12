using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Model.Enums
{
    public enum IODeviceControls
    {
        InterruptEnable = 1 << 0,
        StatusEnable = 1 << 1,
    }
}
