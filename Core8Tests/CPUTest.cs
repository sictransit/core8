using Core8;
using Core8.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Core8Tests
{
    [TestClass]
    public class CPUTest
    {
        private PDP pdp;

        [TestInitialize]
        public void Initialize()
        {
            pdp = new PDP();
        }

        private static void DumpMemory(PDP pdp)
        {
            for (uint i = 0; i < pdp.Memory.Size; i++)
            {
                var data = pdp.Memory.Read(i);

                if (Decoder.TryDecode(data, out var instruction))
                {
                    Trace.WriteLine($"{i.ToOctalString()}: {instruction}");
                }
                else
                {
                    Trace.WriteLine($"{i.ToOctalString()}: {data.ToOctalString()}");
                }
            }
        }

        private static void LoadRIMHighSpeed(PDP pdp)
        {
            pdp.Load8(7756);

            pdp.Deposit8(6014);
            pdp.Deposit8(6011);
            pdp.Deposit8(5357);
            pdp.Deposit8(6016);
            pdp.Deposit8(7106);
            pdp.Deposit8(7006);
            pdp.Deposit8(7510);
            pdp.Deposit8(5374); // 5357
            pdp.Deposit8(7006);
            pdp.Deposit8(6011);
            pdp.Deposit8(5367);
            pdp.Deposit8(6016);
            pdp.Deposit8(7420);
            pdp.Deposit8(3776);
            pdp.Deposit8(3376);
            pdp.Deposit8(5357);
            pdp.Deposit8(0);
            pdp.Deposit8(0);

            pdp.Load8(7756);
        }

        private static void LoadRIMLowSpeed(PDP pdp)
        {
            pdp.Load8(7756);

            pdp.Deposit8(6032);
            pdp.Deposit8(6031);
            pdp.Deposit8(5357);
            pdp.Deposit8(6036);
            pdp.Deposit8(7106);
            pdp.Deposit8(7006);
            pdp.Deposit8(7510);
            pdp.Deposit8(5357);
            pdp.Deposit8(7006);
            pdp.Deposit8(6031);
            pdp.Deposit8(5367);
            pdp.Deposit8(6034);
            pdp.Deposit8(7420);
            pdp.Deposit8(3776);
            pdp.Deposit8(3376);
            pdp.Deposit8(5356);

            pdp.Load8(7756);
        }

        [TestMethod]
        public void TestLoadTape()
        {
            var bin = File.ReadAllBytes(@"Tapes/dec-08-lbaa-pm_5-10-67.bin");

            pdp.LoadTape(bin);

            Assert.IsFalse(pdp.Reader.IsReaderFlagSet);
            Assert.IsTrue(pdp.Reader.IsTapeLoaded);
        }

        [TestMethod]
        public void TestBIN()
        {
            LoadRIMHighSpeed(pdp); // Toggle RIM loader

            var bin = File.ReadAllBytes(@"Tapes/dec-08-lbaa-pm_5-10-67.bin"); // Load a paper tape image from 1967 from disk.

            pdp.LoadTape(bin); // Load tape

            pdp.Start(waitForHalt: false); // Run! RIM loader won't HLT.

            while (pdp.Reader.IsTapeLoaded) // While there is tape to be read ...
            {
                Thread.Sleep(200);
            }

            pdp.Stop(); // HLT!

            Assert.AreEqual(5301u.ToDecimal(), pdp.Memory.Read(4095)); // Verify a JMP @ end of RAM

            DumpMemory(pdp);

            pdp.Load8(7777);

            pdp.Start();
        }


        [TestMethod]
        public void TestIAC()
        {
            var length = 10;

            pdp.Load8(0000);

            for (int i = 0; i < length; i++)
            {
                pdp.Deposit8(7001);
            }

            pdp.Deposit8(7402);

            pdp.Load8(0000);

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
