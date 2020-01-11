using System;

namespace Core8.Model.Enums
{
    [Flags]
    public enum Group3OpCodes : uint
    {
        MQL = 1 << 4,
        MQA = 1 << 6,
        CLA = 1 << 7,
    }
}
