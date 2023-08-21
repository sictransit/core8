using System;
using System.Drawing;

namespace Core8.Peripherals.Teletype;

public class PunchSettings
{
    public PunchSettings() : this(Color.LightYellow)
    {

    }

    public PunchSettings(Color paperColor, int wrap = 80, bool trimLeader = true, int nullPadding = 12)
    {
        if (wrap < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(wrap));
        }

        if (nullPadding < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(nullPadding));
        }

        PaperColor = paperColor;
        Wrap = wrap;
        TrimLeader = trimLeader;
        NullPadding = nullPadding;
    }

    public int Wrap { get; }

    public bool TrimLeader { get; }

    public Color PaperColor { get; }

    public int NullPadding { get; }
}
