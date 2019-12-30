using Core8.Model.Extensions;
using Core8.Tests.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;

namespace Core8.Tests
{
    [TestClass]
    public class Group2ORInstructionTests : InstructionTestsBase
    {
        [TestMethod]
        public void SMA_CLA_Negative()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7700); // SMA, CLA
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetAccumulator(0b_100_000_000_000);

            PDP.Start();

            Assert.AreEqual((uint)131, PDP.Registers.IF_PC.Address);
            Assert.AreEqual((uint)0, PDP.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void SMA_Negative()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7500); // SMA
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetAccumulator(0b_100_000_000_000);

            PDP.Start();

            Assert.AreEqual((uint)131, PDP.Registers.IF_PC.Address);
            Assert.AreEqual((uint)0b_100_000_000_000, PDP.Registers.LINK_AC.Accumulator);
        }
        
        [TestMethod]
        public void SMA_CLA_Zero()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7700); // SMA, CLA
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetAccumulator(0b_000_000_000_000);

            PDP.Start();

            Assert.AreEqual((uint)130, PDP.Registers.IF_PC.Address);
            Assert.AreEqual((uint)0, PDP.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void SMA_CLA_Positive()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7700); // SMA, CLA
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetAccumulator(0b_010_000_000_000);

            PDP.Start();

            Assert.AreEqual((uint)130, PDP.Registers.IF_PC.Address);
            Assert.AreEqual((uint)0, PDP.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void SZA_Zero()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7440); // SZA
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetAccumulator(0);

            PDP.Start();

            Assert.AreEqual((uint)131, PDP.Registers.IF_PC.Address);            
        }

        [TestMethod]
        public void SZA_NonZero()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7440); // SZA
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetAccumulator(1);

            PDP.Start();

            Assert.AreEqual((uint)130, PDP.Registers.IF_PC.Address);
        }

        [TestMethod]
        public void SNL_NonZero()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7420); // SNL
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetLink(1);

            PDP.Start();

            Assert.AreEqual((uint)131, PDP.Registers.IF_PC.Address);
        }

        [TestMethod]
        public void SNL_Zero()
        {
            PDP.Load8(0200);

            PDP.Deposit8(7420); // SNL
            PDP.Deposit8(7402); // HLT
            PDP.Deposit8(7402); // HLT

            PDP.Load8(0200);

            PDP.Registers.LINK_AC.SetLink(0);

            PDP.Start();

            Assert.AreEqual((uint)130, PDP.Registers.IF_PC.Address);
        }
    }
}
