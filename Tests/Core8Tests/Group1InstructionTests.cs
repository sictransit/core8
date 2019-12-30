using Core8.Model.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;

namespace Core8.Tests
{
    [TestClass]
    public class Group1InstructionTests
    {
        private PDP pdp;

        [TestInitialize]
        public void Initialize()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();

            pdp = new PDP();
        }

        [TestMethod]
        public void CLA()
        {
            pdp.Load8(0200);

            pdp.Deposit8(7200); // CLA
            pdp.Deposit8(7402); // HLT

            pdp.Load8(0200);

            pdp.Registers.LINK_AC.SetAccumulator(0b_111_111_111_111);

            pdp.Start();

            Assert.AreEqual((uint)0, pdp.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void CLL()
        {
            pdp.Load8(0200);

            pdp.Deposit8(7100); // CLL
            pdp.Deposit8(7402); // HLT

            pdp.Load8(0200);

            pdp.Registers.LINK_AC.SetLink(1);

            pdp.Start();

            Assert.AreEqual((uint)0, pdp.Registers.LINK_AC.Link);
        }

        [TestMethod]
        public void CMA()
        {
            pdp.Load8(0200);

            pdp.Deposit8(7040); // CMA
            pdp.Deposit8(7402); // HLT

            pdp.Load8(0200);

            pdp.Registers.LINK_AC.SetAccumulator(0);

            pdp.Start();

            Assert.AreEqual((uint)4095, pdp.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void CML()
        {
            pdp.Load8(0200);

            pdp.Deposit8(7020); // CML
            pdp.Deposit8(7402); // HLT

            pdp.Load8(0200);

            pdp.Registers.LINK_AC.SetLink(0);

            pdp.Start();

            Assert.AreEqual((uint)1, pdp.Registers.LINK_AC.Link);
        }

        [TestMethod]
        public void IAC()
        {
            pdp.Load8(0200);

            pdp.Deposit8(7001); // IAC
            pdp.Deposit8(7402); // HLT

            pdp.Load8(0200);

            pdp.Registers.LINK_AC.SetLink(0);
            pdp.Registers.LINK_AC.SetAccumulator(4095);

            pdp.Start();

            Assert.AreEqual((uint)1, pdp.Registers.LINK_AC.Link);
            Assert.AreEqual((uint)0, pdp.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void RAR()
        {
            pdp.Load8(0200);

            pdp.Deposit8(7010); // RAR
            pdp.Deposit8(7402); // HLT

            pdp.Load8(0200);

            pdp.Registers.LINK_AC.SetLink(1);
            pdp.Registers.LINK_AC.SetAccumulator(0b_010_101_010_101);

            pdp.Start();

            Assert.AreEqual((uint)1, pdp.Registers.LINK_AC.Link);
            Assert.AreEqual((uint)0b_101_010_101_010, pdp.Registers.LINK_AC.Accumulator);
        }
        
        [TestMethod]
        public void RAL()
        {
            pdp.Load8(0200);

            pdp.Deposit8(7004); // RAL
            pdp.Deposit8(7402); // HLT

            pdp.Load8(0200);

            pdp.Registers.LINK_AC.SetLink(0);
            pdp.Registers.LINK_AC.SetAccumulator(0b_101_010_101_010);

            pdp.Start();

            Assert.AreEqual((uint)1, pdp.Registers.LINK_AC.Link);
            Assert.AreEqual((uint)0b_010_101_010_100, pdp.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void RTR()
        {
            pdp.Load8(0200);

            pdp.Deposit8(7012); // RTR
            pdp.Deposit8(7402); // HLT

            pdp.Load8(0200);

            pdp.Registers.LINK_AC.SetLink(0);
            pdp.Registers.LINK_AC.SetAccumulator(0b_100_000_000_010);

            pdp.Start();

            Assert.AreEqual((uint)1, pdp.Registers.LINK_AC.Link);
            Assert.AreEqual((uint)0b_001_000_000_000, pdp.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void RTL()
        {
            pdp.Load8(0200);

            pdp.Deposit8(7006); // RTL
            pdp.Deposit8(7402); // HLT

            pdp.Load8(0200);

            pdp.Registers.LINK_AC.SetLink(0);
            pdp.Registers.LINK_AC.SetAccumulator(0b_010_000_000_001);

            pdp.Start();

            Assert.AreEqual((uint)1, pdp.Registers.LINK_AC.Link);
            Assert.AreEqual((uint)0b_000_000_000_100, pdp.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void BSW()
        {
            pdp.Load8(0200);

            pdp.Deposit8(7042); // CMA, BSW
            pdp.Deposit8(7402); // HLT

            pdp.Load8(0200);

            pdp.Registers.LINK_AC.SetLink(1);
            pdp.Registers.LINK_AC.SetAccumulator(0b_000_000_111_111);

            pdp.Start();

            Assert.AreEqual((uint)1, pdp.Registers.LINK_AC.Link);
            Assert.AreEqual((uint)0b_000_000_111_111, pdp.Registers.LINK_AC.Accumulator);
        }

    }
}
