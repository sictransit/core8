using Core8.Extensions;
using Core8.Peripherals.RX8E.Declarations;
using Core8.Peripherals.RX8E.Media.Abstract;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace Core8.Peripherals.RX8E.Media;

internal class Disk : MediaBase
{
    public Disk(int number) : base(number)
    {

    }

    public void LoadFromArray(byte[] data)
    {
        IEnumerable<byte>[] blocks = data.ChunkBy(DiskLayout.BLOCK_SIZE).ToArray();

        int cnt = 0;

        foreach (Track track in Tracks.Values.OrderBy(x => x.Number))
        {
            foreach (Sector sector in track.Sectors.Values.OrderBy(x => x.Number))
            {
                sector.Data = blocks[cnt++].Select(x => x).ToArray();
            }
        }

        if (cnt != blocks.Length)
        {
            Log.Warning("Disk image didn't match disk size.");
        }
    }

    public Dictionary<int, Track> Tracks { get; private set; }

    public override void Format()
    {
        Tracks = new Dictionary<int, Track>();

        for (int i = DiskLayout.FIRST_TRACK; i <= DiskLayout.LAST_TRACK; i++)
        {
            Track track = new(i);
            track.Format();
            Tracks.Add(track.Number, track);
        }
    }
}
