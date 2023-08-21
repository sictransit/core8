using Core8.Peripherals.RX8E.Declarations;
using Core8.Peripherals.RX8E.Media.Abstract;
using System.Collections.Generic;

namespace Core8.Peripherals.RX8E.Media;

internal class Track : MediaBase
{
    public Track(int number) : base(number)
    {

    }

    public Dictionary<int, Sector> Sectors { get; private set; }

    public override void Format()
    {
        Sectors = new Dictionary<int, Sector>();

        for (int i = DiskLayout.FIRST_SECTOR; i <= DiskLayout.LAST_SECTOR; i++)
        {
            Sector sector = new(i);
            sector.Format();
            Sectors.Add(sector.Number, sector);
        }
    }
}
