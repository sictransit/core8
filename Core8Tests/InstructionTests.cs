using Core8;
using Core8.Extensions;
using Core8.Instructions.MemoryReference;
using Core8.Instructions.Microcoded;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core8Tests
{
    [TestClass]
    public class InstrucitonTests
    {
        private Processor proc;

        [TestInitialize]
        public void Initialize()
        {
            proc = new Processor(new Memory(4096));
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

        


    }
}
