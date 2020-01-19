using Core8.Model.Extensions;
using Core8.Model.Instructions;
using Core8.Tests.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core8.Tests
{
    [TestClass]
    public class MemoryManagementInstructionTests : InstructionTestsBase
    {

        [TestMethod]
        public void TestOpCodes()
        {
            PDP.Load8(0200);
            PDP.Deposit8(6201);

            //PDP.Processor.Fetch()

        }      
    }
}
