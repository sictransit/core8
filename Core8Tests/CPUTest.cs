using Core8;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core8Tests
{
    [TestClass]
    public class CPUTest
    {
        [TestMethod]
        public void TestRIM()
        {
            var ram = new Memory(4096);
            var cpu = new Processor(ram);

            cpu.Load8(7756);
            
            cpu.Deposit8(6014);
            cpu.Deposit8(6011);           
            cpu.Deposit8(5357);
            cpu.Deposit8(6016);
            cpu.Deposit8(7106);
            cpu.Deposit8(7006);
            cpu.Deposit8(7510);
            cpu.Deposit8(5357);
            cpu.Deposit8(7006);
            cpu.Deposit8(6011);
            cpu.Deposit8(5367);
            cpu.Deposit8(6016);
            cpu.Deposit8(7420);
            cpu.Deposit8(3776);
            cpu.Deposit8(3376);
            cpu.Deposit8(5357);
            cpu.Deposit8(0);
            cpu.Deposit8(0);

            cpu.Load8(7756);

            cpu.Run();
        }

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
