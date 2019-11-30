using Core8;
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

            cpu.Deposit(new TAD(0b011));
            cpu.Deposit(new AND(0b100));
            cpu.Deposit(new HLT());
            cpu.Deposit(0b_0000_1111_0000);
            cpu.Deposit(0b_1111_1111_1111);

            cpu.Run();

            Assert.AreEqual(0x0f0u, cpu.Accumulator);

        }

        [TestMethod]
        public void TestIAC()
        {
            var ram = new Memory(4096);
            var cpu = new Processor(ram);

            var length = 5;

            for (int i = 0; i < length; i++)
            {
                cpu.Deposit(new IAC());
            }

            cpu.Deposit(new HLT());

            cpu.Run();

            Assert.AreEqual((uint)length, cpu.Accumulator);
        }
    }
}
