using Core8.Tests.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core8.Tests
{
    [TestClass]
    public class Group1InstructionTests : InstructionTestsBase
    {
        [TestMethod]
        public void CLA()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7200); // CLA
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetAccumulator(0b_111_111_111_111);

            PDP.Continue();

            Assert.AreEqual((uint)0, PDP.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void CLL()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7100); // CLL
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetLink(1);

            PDP.Continue();

            Assert.AreEqual((uint)0, PDP.Registers.LINK_AC.Link);
        }

        [TestMethod]
        public void CMA()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7040); // CMA
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetAccumulator(0);

            PDP.Continue();

            Assert.AreEqual((uint)4095, PDP.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void CML()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7020); // CML
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetLink(0);

            PDP.Continue();

            Assert.AreEqual((uint)1, PDP.Registers.LINK_AC.Link);
        }

        [TestMethod]
        public void IAC()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7001); // IAC
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetLink(0);
            PDP.Registers.LINK_AC.SetAccumulator(4095);

            PDP.Continue();

            Assert.AreEqual((uint)1, PDP.Registers.LINK_AC.Link);
            Assert.AreEqual((uint)0, PDP.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void RAR()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7010); // RAR
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetLink(1);
            PDP.Registers.LINK_AC.SetAccumulator(0b_010_101_010_101);

            PDP.Continue();

            Assert.AreEqual((uint)1, PDP.Registers.LINK_AC.Link);
            Assert.AreEqual((uint)0b_101_010_101_010, PDP.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void RAL()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7004); // RAL
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetLink(0);
            PDP.Registers.LINK_AC.SetAccumulator(0b_101_010_101_010);

            PDP.Continue();

            Assert.AreEqual((uint)1, PDP.Registers.LINK_AC.Link);
            Assert.AreEqual((uint)0b_010_101_010_100, PDP.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void RTR()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7012); // RTR
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetLink(0);
            PDP.Registers.LINK_AC.SetAccumulator(0b_100_000_000_010);

            PDP.Continue();

            Assert.AreEqual((uint)1, PDP.Registers.LINK_AC.Link);
            Assert.AreEqual((uint)0b_001_000_000_000, PDP.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void RTL()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7006); // RTL
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetLink(0);
            PDP.Registers.LINK_AC.SetAccumulator(0b_010_000_000_001);

            PDP.Continue();

            Assert.AreEqual((uint)1, PDP.Registers.LINK_AC.Link);
            Assert.AreEqual((uint)0b_000_000_000_100, PDP.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void BSW()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7042); // CMA, BSW
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetLink(1);
            PDP.Registers.LINK_AC.SetAccumulator(0b_000_000_111_111);

            PDP.Continue();

            Assert.AreEqual((uint)1, PDP.Registers.LINK_AC.Link);
            Assert.AreEqual((uint)0b_000_000_111_111, PDP.Registers.LINK_AC.Accumulator);
        }

    }
}
