using Core8;
using Core8.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core8Tests
{
    [TestClass]
    public class InstructionTests
    {
        private Processor proc;
        private Memory ram;

        [TestInitialize]
        public void Initialize()
        {
            ram = new Memory(4096);
            proc = new Processor(ram);
        }

        [TestMethod]
        public void AND()
        {
            proc.Load8(0000);

            proc.Deposit8(7200); // CLA
            proc.Deposit8(1005); // TAD
            proc.Deposit8(0006); // AND
            proc.Deposit8(0007); // AND
            proc.Deposit8(7402); // HLT
            proc.Deposit8(7777);
            proc.Deposit8(0777);
            proc.Deposit8(7770);

            proc.Load8(0000);

            proc.Run();

            Assert.AreEqual(0770u.ToDecimal(), proc.Accumulator);
        }

        [TestMethod]
        public void TAD_NC()
        {
            proc.Load8(0000);

            proc.Deposit8(7200); // CLA
            proc.Deposit8(1003); // TAD
            proc.Deposit8(7402); // HLT
            proc.Deposit8(1234);

            proc.Load8(0000);

            proc.Run();

            Assert.AreEqual(1234u.ToDecimal(), proc.Accumulator);
        }

        [TestMethod]
        public void TAD_WC()
        {
            proc.Load8(0000);

            proc.Deposit8(7200); // CLA
            proc.Deposit8(1004); // TAD
            proc.Deposit8(1005); // TAD
            proc.Deposit8(7402); // HLT
            proc.Deposit8(7777);
            proc.Deposit8(0001);

            proc.Load8(0000);

            proc.Run();

            Assert.AreEqual(0u, proc.Accumulator);
            Assert.AreEqual(1u, proc.Link);
        }

        [TestMethod]
        public void ISZ_NZ()
        {
            proc.Load8(0000);

            proc.Deposit8(2003); // ISZ
            proc.Deposit8(7402); // HLT
            proc.Deposit8(7402); // HLT
            proc.Deposit8(7776);

            proc.Load8(0000);

            proc.Run();

            Assert.AreEqual(2u, proc.ProgramCounterWord);
        }

        [TestMethod]
        public void ISZ_WZ()
        {
            proc.Load8(0000);

            proc.Deposit8(2003); // ISZ
            proc.Deposit8(7402); // HLT
            proc.Deposit8(7402); // HLT
            proc.Deposit8(7777);

            proc.Load8(0000);

            proc.Run();

            Assert.AreEqual(3u, proc.ProgramCounterWord);
        }

        [TestMethod]
        public void DCA()
        {
            proc.Load8(0000);

            proc.Deposit8(7300); // CLA, CLL
            proc.Deposit8(1006); // TAD
            proc.Deposit8(1007); // TAD
            proc.Deposit8(1006); // TAD
            proc.Deposit8(3010); // DCA
            proc.Deposit8(7402); // HLT
            proc.Deposit8(7777);
            proc.Deposit8(0001);

            proc.Load8(0000);

            proc.Run();

            Assert.AreEqual(0u, proc.Accumulator);
            Assert.AreEqual(1u, proc.Link);

            Assert.AreEqual(7777u.ToDecimal(), ram.Read(0008u));
        }

        [TestMethod]
        public void JMS()
        {
            proc.Load8(0000);

            proc.Deposit8(4002); // JMS
            proc.Deposit8(7402);
            proc.Deposit8(7402);
            proc.Deposit8(7402); // HLT

            proc.Load8(0000);

            proc.Run();

            Assert.AreEqual(4u, proc.ProgramCounterWord);

            Assert.AreEqual(0001u, ram.Read(0002u));
        }

        [TestMethod]
        public void JMP()
        {
            proc.Load8(0000);

            proc.Deposit8(5002); // JMP
            proc.Deposit8(7402);
            proc.Deposit8(7402);

            proc.Load8(0000);

            proc.Run();

            Assert.AreEqual(3u, proc.ProgramCounterWord);
        }
    }
}
