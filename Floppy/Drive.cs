using Core8.Floppy.Interfaces;
using Serilog;

namespace Core8.Floppy
{
    internal class Drive : IDrive
    {
        private int trackAddress = 0;
        private int sectorAddress = 0;        

        private const int FirstTrack = 0;
        private const int LastTrack = 76;
        private const int FirstSector = 1;
        private const int LastSector = 26;
        private const int BlockSize = 128;

        public void SetSectorAddress(int sector)
        {
            sectorAddress = sector & 0b_000_001_111_111;

            if (sectorAddress < FirstSector || sectorAddress > LastSector)
            {
                Log.Warning($"Bad sector address: {sectorAddress}");
            }
        }

        public void SetTrackAddress(int track)
        {
            trackAddress = track & 0b_000_011_111_111;

            if (trackAddress < FirstTrack || trackAddress > LastTrack)
            {
                Log.Warning($"Bad track address: {trackAddress}");
            }
        }
    }
}
