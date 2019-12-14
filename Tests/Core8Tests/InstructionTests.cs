using Core8.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;

namespace Core8.Tests
{
    [TestClass]
    public class InstructionTests
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
        public void AND()
        {
            pdp.Load8(0200);

            pdp.Deposit8(7200); // CLA
            pdp.Deposit8(1205); // TAD Z
            pdp.Deposit8(0206); // AND
            pdp.Deposit8(0207); // AND
            pdp.Deposit8(7402); // HLT
            pdp.Deposit8(7777);
            pdp.Deposit8(0777);
            pdp.Deposit8(7770);

            pdp.Load8(0200);

            pdp.Start();

            Assert.AreEqual(0770u.ToDecimal(), pdp.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void TAD_NC()
        {
            pdp.Load8(0200);

            pdp.Deposit8(7200); // CLA
            pdp.Deposit8(1203); // TAD
            pdp.Deposit8(7402); // HLT
            pdp.Deposit8(1234);

            pdp.Load8(0200);

            pdp.Start();

            Assert.AreEqual(1234u.ToDecimal(), pdp.Registers.LINK_AC.Accumulator);
        }

        [TestMethod]
        public void TAD_WC()
        {
            pdp.Load8(0200);

            pdp.Deposit8(7200); // CLA
            pdp.Deposit8(1204); // TAD
            pdp.Deposit8(1205); // TAD
            pdp.Deposit8(7402); // HLT
            pdp.Deposit8(7777);
            pdp.Deposit8(0001);

            pdp.Load8(0200);

            pdp.Start();

            Assert.AreEqual(0u, pdp.Registers.LINK_AC.Accumulator);
            Assert.AreEqual(1u, pdp.Registers.LINK_AC.Link);
        }

        [TestMethod]
        public void ISZ_NZ()
        {
            pdp.Load8(0200);

            pdp.Deposit8(2203); // ISZ
            pdp.Deposit8(7402); // HLT
            pdp.Deposit8(7402); // HLT
            pdp.Deposit8(7776);

            pdp.Load8(0200);

            pdp.Start();

            Assert.AreEqual(2u, pdp.Registers.IF_PC.Word);
        }

        [TestMethod]
        public void ISZ_WZ()
        {
            pdp.Load8(0200);

            pdp.Deposit8(2203); // ISZ
            pdp.Deposit8(7402); // HLT
            pdp.Deposit8(7402); // HLT
            pdp.Deposit8(7777);

            pdp.Load8(0200);

            pdp.Start();

            Assert.AreEqual(3u, pdp.Registers.IF_PC.Word);
        }

        [TestMethod]
        public void DCA()
        {
            pdp.Load8(0200);

            pdp.Deposit8(7300); // CLA, CLL
            pdp.Deposit8(1206); // TAD
            pdp.Deposit8(3210); // DCA
            pdp.Deposit8(1206); // TAD
            pdp.Deposit8(1207); // TAD                        
            pdp.Deposit8(7402); // HLT
            pdp.Deposit8(7777);
            pdp.Deposit8(0001);

            pdp.Load8(0200);

            pdp.Start();

            Assert.AreEqual(0u, pdp.Registers.LINK_AC.Accumulator);
            Assert.AreEqual(1u, pdp.Registers.LINK_AC.Link);

            Assert.AreEqual(7777u.ToDecimal(), pdp.Memory.Read(0210u.ToDecimal()));
        }

        [TestMethod]
        public void JMS()
        {
            pdp.Load8(0200);

            pdp.Deposit8(4202); // JMS
            pdp.Deposit8(0000);
            pdp.Deposit8(7402);
            pdp.Deposit8(7402); // HLT

            pdp.Load8(0200);

            pdp.Start();

            Assert.AreEqual(4u, pdp.Registers.IF_PC.Word);

            Assert.AreEqual(0201u.ToDecimal(), pdp.Memory.Read(0202u.ToDecimal()));
        }

        [TestMethod]
        public void JMP()
        {
            pdp.Load8(0200);

            pdp.Deposit8(5202); // JMP
            pdp.Deposit8(7402);
            pdp.Deposit8(7402);

            pdp.Load8(0200);

            pdp.Start();

            Assert.AreEqual(3u, pdp.Registers.IF_PC.Word);
        }
    }
}
