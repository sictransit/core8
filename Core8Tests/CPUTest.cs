using Core8;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core8Tests
{
    [TestClass]
    public class CPUTest
    {


        [TestMethod]
        public void TestIAC()
        {
            var ram = new Memory(4096);
            var cpu = new Processor(ram);

            var length = 10;

            cpu.Load8(0000);

            for (int i = 0; i < length; i++)
            {
                cpu.Deposit8(7001);
            }

            cpu.Deposit8(7402);

            cpu.Load8(0000);

            cpu.Run();

            Assert.IsTrue(cpu.Accumulator == length);
        }

        [TestMethod]
        public void TestPaging()
        {
            var ram = new Memory(4096);
            var cpu = new Processor(ram);

            cpu.Load8(0200);

            cpu.Deposit8(7300);
            cpu.Deposit8(1205);
            cpu.Deposit8(1206);
            cpu.Deposit8(3207);
            cpu.Deposit8(7402);
            cpu.Deposit8(0002);
            cpu.Deposit8(0003);

            cpu.Load8(0200);
            cpu.Run();

            cpu.Load8(0207);
            cpu.Exam();

            Assert.AreEqual(5u, cpu.Accumulator);
        }

        [TestMethod]
        public void TestAddition()
        {
            var ram = new Memory(4096);
            var cpu = new Processor(ram);

            cpu.Load8(0000);

            cpu.Deposit8(7300);
            cpu.Deposit8(1005);
            cpu.Deposit8(1006);
            cpu.Deposit8(3007);
            cpu.Deposit8(7402);
            cpu.Deposit8(0002);
            cpu.Deposit8(0003);

            cpu.Load8(0000);
            cpu.Run();

            cpu.Load8(0007);
            cpu.Exam();

            Assert.AreEqual(5u, cpu.Accumulator);
        }

    }
}
