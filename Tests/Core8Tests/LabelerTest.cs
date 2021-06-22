using Core8.Peripherals.Teletype;
using Core8.Tests.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
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

            const string text = "PDP-8/E PAPER TAPE LABELER";

            PDP.Continue(false);

            var teletype = PDP.CPU.Teletype;

            foreach (var c in text)
            {
                while (teletype.InputFlag)
                {
                    Thread.Sleep(100);
                }

                Thread.Sleep(100);

                teletype.Type((byte)c);

                while (!teletype.OutputFlag)
                {
                    Thread.Sleep(100);
                }

                Thread.Sleep(100);
            }

            var punch = new SVGPunch(new PunchSettings(Color.LightGoldenrodYellow, 0));

            const string comment = "this is a comment";
            const string title = "this is the title";

            var paper = punch.Punch(teletype.Output.ToArray(), title, comment);

            Assert.IsNotNull(paper);
            Assert.IsTrue(paper.Contains(comment));
            Assert.IsTrue(paper.Contains(title));

            PDP.Halt();
        }
    }
}
