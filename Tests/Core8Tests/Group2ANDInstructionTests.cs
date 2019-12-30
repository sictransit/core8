using Core8.Tests.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core8.Tests
{
    [TestClass]
    public class Group2ANDInstructionTests : InstructionTestsBase
    {
        [TestMethod]
        public void SPA_CLA_NonNegative()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7710); // SPA, CLA
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetAccumulator(0b_011_111_111_111);

            PDP.Start();

            Assert.AreEqual((uint)131, PDP.Registers.IF_PC.Address);
            Assert.AreEqual((uint)0, PDP.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void SPA_CLA_Negative()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7710); // SPA, CLA
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetAccumulator(0b_111_111_111_111);

            PDP.Start();

            Assert.AreEqual((uint)130, PDP.Registers.IF_PC.Address);
            Assert.AreEqual((uint)0, PDP.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void SNA_CLA_NonZero()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7650); // SNA, CLA
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetAccumulator(0b_000_000_000_001);

            PDP.Start();

            Assert.AreEqual((uint)131, PDP.Registers.IF_PC.Address);
            Assert.AreEqual((uint)0, PDP.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void SZL_CLA_NonZero()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7630); // SZL, CLA
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetLink(1);

            PDP.Start();

            Assert.AreEqual((uint)130, PDP.Registers.IF_PC.Address);
            Assert.AreEqual((uint)1, PDP.Registers.LINK_AC.Link);
        }

        [TestMethod]
        public void SZL_CLA_Zero()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7630); // SZL, CLA
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetLink(0);

            PDP.Start();

            Assert.AreEqual((uint)131, PDP.Registers.IF_PC.Address);
            Assert.AreEqual((uint)0, PDP.Registers.LINK_AC.Link);
        }

        [TestMethod]
        public void SNA_CLA_Zero()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7650); // SNA, CLA
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetAccumulator(0b_000_000_000_000);

            PDP.Start();

            Assert.AreEqual((uint)130, PDP.Registers.IF_PC.Address);
            Assert.AreEqual((uint)0, PDP.Registers.LINK_AC.Accumulator);
        }
    }
}
