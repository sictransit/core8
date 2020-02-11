using System.Net.Http;

namespace Core8
{
    public static class TINT
    {
        public static void Play()
        {
            var pdp = new PDP();

            pdp.Clear();

            using var httpClient = new HttpClient();

            pdp.LoadPaperTape(httpClient.GetByteArrayAsync(@"https://github.com/PontusPih/TINT8/releases/download/v0.1.0-alpha/tint.bin").Result);

            pdp.Clear();

            pdp.Load8(0200);

            pdp.Continue(waitForHalt: true);
        }
    }
}
