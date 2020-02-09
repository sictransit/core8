using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Core8
{
    public static class TINT
    {
        public static void Play()
        {
            var pdp = new PDP();

            pdp.Clear();

            pdp.LoadPaperTape(new HttpClient().GetByteArrayAsync(@"https://github.com/PontusPih/TINT8/releases/download/v0.1.0-alpha/tint.bin").Result);

            pdp.Clear();

            pdp.Load8(0200);

            pdp.Continue(waitForHalt: true);
        }
    }
}
