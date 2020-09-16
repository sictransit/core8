using Core8.Peripherals.Floppy.Declarations;
using Core8.Peripherals.Floppy.Media.Abstract;
using System.Collections.Generic;

namespace Core8.Peripherals.Floppy.Media
{
    internal class Track : MediaBase
    {
        public Track(int number) : base(number)
        {

        }

        public Dictionary<int, Sector> Sectors { get; private set; }

        public override void Format()
        {
            Sectors = new Dictionary<int, Sector>();

            for (int i = DiskLayout.FirstSector; i <= DiskLayout.LastSector; i++)
            {
                var sector = new Sector(i);
                sector.Format();
                Sectors.Add(sector.Number, sector);
            }
        }
    }
}
