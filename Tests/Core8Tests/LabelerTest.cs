using Core8.Tests.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Core8.Tests
{
    [TestClass]
    public class LabelerTest : PDPTestsBase
    { 
        [TestMethod]
        public async Task TestLabeler()
        {
            using var httpClient = new HttpClient();

            var bin = await httpClient.GetByteArrayAsync(@"https://raw.githubusercontent.com/sictransit/paper-tape-labeler/main/src/FontAssembler/asm/glyphs.bin");

            PDP.LoadPaperTape(bin);

            PDP.Load8(0200);

            var text = Enumerable.Range(32, 64).Select(x => (char)x);

            PDP.Continue(false);

            var sw = new Stopwatch();

            foreach (var c in text)
            {
                while (PDP.CPU.Teletype.InputFlag)
                {
                    Thread.Sleep(100);
                }

                var length = PDP.CPU.Teletype.Printout.Length;

                sw.Restart();

                PDP.CPU.Teletype.Type((byte)c);

                while (sw.ElapsedMilliseconds < 1000 && length == PDP.CPU.Teletype.Printout.Length)
                {
                    Thread.Sleep(100);
                }

                Assert.AreNotEqual(length, PDP.CPU.Teletype.Printout.Length);
            }            

            PDP.Halt();
        }
    }
}
