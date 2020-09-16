using System;

namespace Core8.Peripherals.Floppy.Declarations
{
    public class Latencies
    {
        public static TimeSpan InitializeTime => TimeSpan.FromMilliseconds(1800);
        public static TimeSpan ReadStatusTime => TimeSpan.FromMilliseconds(250);
        public static TimeSpan CommandTime => TimeSpan.FromMilliseconds(100);
        public static TimeSpan AverageAccessTime => TimeSpan.FromMilliseconds(488);
    }
}
