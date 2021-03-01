using Core8.Tests.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Core8.Peripherals.Teletype;

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

            //var text = Enumerable.Range(32, 64).Select(x => (char)x);

            var text = "PAPER TAPE LABELER";

            PDP.Continue(false);

            var sw = new Stopwatch();

            var teletype = PDP.CPU.Teletype;

            foreach (var c in text)
            {
                while (teletype.InputFlag)
                {
                    Thread.Sleep(100);
                }

                var length = teletype.Output.Count;

                sw.Restart();

                teletype.Type((byte)c);

                while (sw.ElapsedMilliseconds < 1000 && length == teletype.Output.Count)
                {
                    Thread.Sleep(100);
                }

                Assert.AreNotEqual(length, teletype.Output.Count);
            }

            var punch = new SVGPunch(new PunchSettings(Color.LightPink, 0));

            var label = punch.Punch(teletype.Output.ToArray(), text);

            Assert.IsNotNull(label);

            PDP.Halt();
        }
    }
}
