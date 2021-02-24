using Core8.Tests.Abstract;
using Core8.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Core8.Tests
{
    [TestClass]
    public class AssemblerTest : PDPTestsBase
    {
        [TestMethod]
        public void TestAssembler()
        {
            var assembler = new Assembler(@"c:\bin\palbart\palbart.exe");

            var result = assembler.TryAssemble(@"Assembler\HelloWorld.asm", out string binFilename);

            Assert.IsTrue(result);

            PDP.Clear();

            PDP.LoadPaperTape(File.ReadAllBytes(binFilename));

            PDP.Load8(0200);

            PDP.Continue(waitForHalt: true);

            Assert.AreEqual("HELLO WORLD!", PDP.CPU.Teletype.Printout);
        }
    }
}
