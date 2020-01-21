using Core8.Tests.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core8.Tests
{
    [TestClass]
    public class MemoryManagementInstructionTests : InstructionTestsBase
    {

        [TestMethod]
        public void TestChangeOpCodes()
        {
            PDP.Load8(0200);
            PDP.Deposit8(6273);

            var i = PDP.Processor.Debug8(0200);

            Assert.IsTrue(i.ToString().Contains("CDF"));
            Assert.IsTrue(i.ToString().Contains("CIF"));
        }

        [TestMethod]
        public void TestReadOpCodes()
        {
            PDP.Load8(0200);
            PDP.Deposit8(6214);
            PDP.Deposit8(6224);
            PDP.Deposit8(6234);
            PDP.Deposit8(6244);

            Assert.IsTrue(PDP.Processor.Debug8(0200).ToString().Contains("RDF"));
            Assert.IsTrue(PDP.Processor.Debug8(0201).ToString().Contains("RIF"));
            Assert.IsTrue(PDP.Processor.Debug8(0202).ToString().Contains("RIB"));
            Assert.IsTrue(PDP.Processor.Debug8(0203).ToString().Contains("RMF"));
            
        }

    }
}
