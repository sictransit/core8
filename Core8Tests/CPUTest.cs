using Core8;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Core8Tests
{
    [TestClass]
    public class CPUTest
    {
        private PDP pdp;
        private Memory ram;

        [TestInitialize]
        public void Initialize()
        {
            ram = new Memory(4096);
            pdp = new PDP(ram);
        }

        [TestMethod]
        public void TestRIM()
        {
            pdp.Load8(7756);

            pdp.Deposit8(6014);
            pdp.Deposit8(6011);
            pdp.Deposit8(5357);
            pdp.Deposit8(6016);
            pdp.Deposit8(7106);
            pdp.Deposit8(7006);
            pdp.Deposit8(7510);
            pdp.Deposit8(5357);
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

            pdp.Start();
        }

        [TestMethod]
        public void TestBIN()
        {
            var bin = File.ReadAllBytes(@"Tapes/dec-08-lbaa-pm_5-10-67.bin");

            pdp.LoadTape(bin);
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

            Assert.IsTrue(pdp.Accumulator == length);
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

            Assert.AreEqual(5u, pdp.Accumulator);
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

            Assert.AreEqual(5u, pdp.Accumulator);
        }

    }
}
