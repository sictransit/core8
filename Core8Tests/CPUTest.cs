using Core8;
using Core8.Extensions;
using Core8.Instructions.MemoryReference;
using Core8.Instructions.Microcoded;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core8Tests
{
    [TestClass]
    public class CPUTest
    {
        [TestMethod]
        public void TestDeposit()
        {
            var ram = new Memory(4096);
            var cpu = new Processor(ram);

            cpu.Load(0000);

            cpu.Deposit(new TAD(0b011));
            cpu.Deposit(new AND(0b100));
            cpu.Deposit(7402);
            cpu.Deposit(0770);
            cpu.Deposit(7707);

            cpu.Load(0000);

            cpu.Run();

            Assert.IsTrue(0700.ToDecimal() == cpu.Accumulator);

        }

        [TestMethod]
        public void TestIAC()
        {
            var ram = new Memory(4096);
            var cpu = new Processor(ram);

            var length = 10;

            cpu.Load(0000);

            for (int i = 0; i < length; i++)
            {
                cpu.Deposit(7001);
            }

            cpu.Deposit(7402);

            cpu.Load(0000);

            cpu.Run();

            Assert.IsTrue(cpu.Accumulator == length);
        }

        [TestMethod]
        public void TestPaging()
        {
            var ram = new Memory(4096);
            var cpu = new Processor(ram);

            cpu.Load(0200);

            cpu.Deposit(7300);
            cpu.Deposit(1205);
            cpu.Deposit(1206);
            cpu.Deposit(3207);
            cpu.Deposit(7402);
            cpu.Deposit(0002);
            cpu.Deposit(0003);

            cpu.Load(0200);
            cpu.Run();

            cpu.Load(0207);
            cpu.Exam();

            Assert.AreEqual(5u, cpu.Accumulator);
        }

        [TestMethod]
        public void TestAddition()
        {
            var ram = new Memory(4096);
            var cpu = new Processor(ram);

            cpu.Load(0000);

            cpu.Deposit(7300);
            cpu.Deposit(1005);
            cpu.Deposit(1006);
            cpu.Deposit(3007);
            cpu.Deposit(7402);
            cpu.Deposit(0002);
            cpu.Deposit(0003);

            cpu.Load(0000);
            cpu.Run();

            cpu.Load(0007);
            cpu.Exam();

            Assert.AreEqual(5u, cpu.Accumulator);
        }

    }
}
