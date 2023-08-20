using Core8.Tests.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;

namespace Core8.Tests
{
    [TestClass]
    public class CPUTests : PDPTestsBase
    {
        [TestMethod]
        public void TestHelloWorld()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7200);
            PDP.Deposit8(7100);
            PDP.Deposit8(1220);
            PDP.Deposit8(3010);
            PDP.Deposit8(7000);
            PDP.Deposit8(1410);
            PDP.Deposit8(7450);
            PDP.Deposit8(7402); //5577
            PDP.Deposit8(4212);
            PDP.Deposit8(5204);
            PDP.Deposit8(0000);
            PDP.Deposit8(6046);
            PDP.Deposit8(6041);
            PDP.Deposit8(5214);
            PDP.Deposit8(7200);
            PDP.Deposit8(5612);
            PDP.Deposit8(0220);
            PDP.Deposit8(0110);
            PDP.Deposit8(0105);
            PDP.Deposit8(0114);
            PDP.Deposit8(0114);
            PDP.Deposit8(0117);
            PDP.Deposit8(0040);
            PDP.Deposit8(0127);
            PDP.Deposit8(0117);
            PDP.Deposit8(0122);
            PDP.Deposit8(0114);
            PDP.Deposit8(0104);
            PDP.Deposit8(0041);
            PDP.Deposit8(0000);

            //pdp.Load8(0177);
            //pdp.Deposit8(7600);

            //PDP.DumpMemory();

            PDP.Clear();

            PDP.Load8(0200);
            PDP.Continue();

            Assert.AreEqual("HELLO WORLD!", PDP.CPU.Teletype.Printout);

            Log.Information(PDP.CPU.Teletype.Printout);
        }

        [TestMethod]
        public void TestBreakpoint()
        {
            PDP.SetBreakpoint8(4);

            PDP.Load8(0);
            PDP.Continue();

            Assert.AreEqual(4, PDP.CPU.Registry.PC.Address);
        }
    }
}
