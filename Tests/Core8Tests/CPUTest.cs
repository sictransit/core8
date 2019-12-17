using Core8.Model.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;

namespace Core8.Tests
{
    [TestClass]
    public class CPUTest
    {
        private PDP pdp;

        [TestInitialize]
        public void Initialize()
        {
            pdp = new PDP();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();
        }

        private static void DumpMemory(PDP pdp)
        {
            for (uint address = 0; address < pdp.Memory.Size; address++)
            {
                var data = pdp.Memory.Read(address, true);

                var instruction = Processor.Decode(address, data, 3, 4);

                if (instruction != null)
                {
                    Log.Debug($"{instruction}");
                }
                else
                {
                    Log.Debug($"{address.ToOctalString()}: {data.ToOctalString()}");
                }
            }
        }


        [TestMethod]
        public void TestHelloWorld()
        {
            pdp.Load8(0200);

            pdp.Deposit8(7200);
            pdp.Deposit8(7100);
            pdp.Deposit8(1220);
            pdp.Deposit8(3010);
            pdp.Deposit8(7000);
            pdp.Deposit8(1410);
            pdp.Deposit8(7450);
            pdp.Deposit8(7402); //5577
            pdp.Deposit8(4212);
            pdp.Deposit8(5204);
            pdp.Deposit8(0000);
            pdp.Deposit8(6046);
            pdp.Deposit8(6041);
            pdp.Deposit8(5214);
            pdp.Deposit8(7200);
            pdp.Deposit8(5612);
            pdp.Deposit8(0220);
            pdp.Deposit8(0110);
            pdp.Deposit8(0105);
            pdp.Deposit8(0114);
            pdp.Deposit8(0114);
            pdp.Deposit8(0117);
            pdp.Deposit8(0040);
            pdp.Deposit8(0127);
            pdp.Deposit8(0117);
            pdp.Deposit8(0122);
            pdp.Deposit8(0114);
            pdp.Deposit8(0104);
            pdp.Deposit8(0041);
            pdp.Deposit8(0000);

            //pdp.Load8(0177);
            //pdp.Deposit8(7600);

            DumpMemory(pdp);

            pdp.Load8(0200);
            pdp.Start();

            Assert.AreEqual("HELLO WORLD!", pdp.Teleprinter.Printout);

            Log.Information(pdp.Teleprinter.Printout);
        }

        [TestMethod]
        public void TestIAC()
        {
            var length = 10;

            pdp.Load8(0200);

            for (int i = 0; i < length; i++)
            {
                pdp.Deposit8(7001);
            }

            pdp.Deposit8(7402);

            pdp.Load8(0200);

            pdp.Start();

            Assert.IsTrue(pdp.Registers.LINK_AC.Accumulator == length);
        }

        [TestMethod]
        public void TestPaging()
        {
            pdp.Load8(0200);

            pdp.Deposit8(7300);
            pdp.Deposit8(1205);
            pdp.Deposit8(1206);
            pdp.Deposit8(3207);
            pdp.Deposit8(7402);
            pdp.Deposit8(0002);
            pdp.Deposit8(0003);

            pdp.Load8(0200);
            pdp.Start();

            pdp.Load8(0207);
            pdp.Exam();

            Assert.AreEqual(5u, pdp.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void TestAddition()
        {
            pdp.Load8(0000);

            pdp.Deposit8(7300);
            pdp.Deposit8(1005);
            pdp.Deposit8(1006);
            pdp.Deposit8(3007);
            pdp.Deposit8(7402);
            pdp.Deposit8(0002);
            pdp.Deposit8(0003);

            pdp.Load8(0000);
            pdp.Start();

            pdp.Load8(0007);
            pdp.Exam();

            Assert.AreEqual(5u, pdp.Registers.LINK_AC.Accumulator);
        }
    }
}
