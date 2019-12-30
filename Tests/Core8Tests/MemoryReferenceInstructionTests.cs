using Core8.Model.Extensions;
using Core8.Tests.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core8.Tests
{
    [TestClass]
    public class MemoryReferenceInstructionTests : InstructionTestsBase
    {
        [TestMethod]
        public void AND()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7200); // CLA
            PDP.Deposit8(1205); // TAD Z
            PDP.Deposit8(0206); // AND
            PDP.Deposit8(0207); // AND
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7777);
            PDP.Deposit8(0777);
            PDP.Deposit8(7770);

            PDP.Load8(0200);

            PDP.Start();

            Assert.AreEqual(0770u.ToDecimal(), PDP.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void TAD_NC()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7200); // CLA
            PDP.Deposit8(1203); // TAD
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(1234);

            PDP.Load8(0200);

            PDP.Start();

            Assert.AreEqual(1234u.ToDecimal(), PDP.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void TAD_WC()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7200); // CLA
            PDP.Deposit8(1204); // TAD
            PDP.Deposit8(1205); // TAD
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7777);
            PDP.Deposit8(0001);

            PDP.Load8(0200);

            PDP.Start();

            Assert.AreEqual(0u, PDP.Registers.LINK_AC.Accumulator);
            Assert.AreEqual(1u, PDP.Registers.LINK_AC.Link);
        }

        [TestMethod]
        public void ISZ_NZ()
        {
            PDP.Load8(0200);

            PDP.Deposit8(2203); // ISZ
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7776);

            PDP.Load8(0200);

            PDP.Start();

            Assert.AreEqual(2u, PDP.Registers.IF_PC.Word);
        }

        [TestMethod]
        public void ISZ_WZ()
        {
            PDP.Load8(0200);

            PDP.Deposit8(2203); // ISZ
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7777);

            PDP.Load8(0200);

            PDP.Start();

            Assert.AreEqual(3u, PDP.Registers.IF_PC.Word);
        }

        [TestMethod]
        public void DCA()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7300); // CLA, CLL
            PDP.Deposit8(1206); // TAD
            PDP.Deposit8(3210); // DCA
            PDP.Deposit8(1206); // TAD
            PDP.Deposit8(1207); // TAD                        
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7777);
            PDP.Deposit8(0001);

            PDP.Load8(0200);

            PDP.Start();

            Assert.AreEqual(0u, PDP.Registers.LINK_AC.Accumulator);
            Assert.AreEqual(1u, PDP.Registers.LINK_AC.Link);

            Assert.AreEqual(7777u.ToDecimal(), PDP.Memory.Examine(0210u.ToDecimal()));
        }

        [TestMethod]
        public void JMS()
        {
            PDP.Load8(0200);

            PDP.Deposit8(4202); // JMS
            PDP.Deposit8(0000);
            PDP.Deposit8(7402);
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Start();

            Assert.AreEqual(4u, PDP.Registers.IF_PC.Word);

            Assert.AreEqual(0201u.ToDecimal(), PDP.Memory.Examine(0202u.ToDecimal()));
        }

        [TestMethod]
        public void JMP()
        {
            PDP.Load8(0200);

            PDP.Deposit8(5202); // JMP
            PDP.Deposit8(7402);
            PDP.Deposit8(7402);

            PDP.Load8(0200);

            PDP.Start();

            Assert.AreEqual(3u, PDP.Registers.IF_PC.Word);
        }
    }
}
