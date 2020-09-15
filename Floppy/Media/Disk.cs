﻿using Core8.Floppy.Declarations;
using Core8.Floppy.Media.Abstract;
using Core8.Model.Extensions;
using Serilog;
using System.Collections.Generic;
using System.Linq;

namespace Core8.Floppy.Media
{
    internal class Disk : MediaBase
    {
        public Disk(int number) : base(number)
        {
            Format();
        }

        public void LoadFromArray(byte[] data)
        {
            var blocks = data.ChunkBy(DiskLayout.BlockSize).ToArray();

            var cnt = 0;

            foreach (var track in Tracks.Values.OrderBy(x => x.Number))
            {
                foreach (var sector in track.Sectors.Values.OrderBy(x => x.Number))
                {
                    sector.Data = blocks[cnt++].Select(x => (byte)x).ToArray();
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

            for (int i = DiskLayout.FirstTrack; i <= DiskLayout.LastTrack; i++)
            {
                var track = new Track(i);
                track.Format();
                Tracks.Add(track.Number, track);
            }
        }
    }
}
