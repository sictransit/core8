using Core8.Tests.Abstract;
using Core8.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Core8.Tests;

[TestClass]
public class AssemblerTest : PDPTestsBase
{
    [TestMethod]
    public void TestAssembler()
    {
        Assembler assembler = new(@"c:\bin\palbart.exe");

        var result = assembler.TryAssemble(@"Assembler\HelloWorld.asm", out var binFilename);

        Assert.IsTrue(result);

        PDP.Clear();

        PDP.LoadPaperTape(File.ReadAllBytes(binFilename));

        PDP.Load8(0200);

        PDP.Continue(waitForHalt: true);

        Assert.AreEqual("HELLO WORLD!", PDP.CPU.PrinterPunch.Printout);
    }
}
