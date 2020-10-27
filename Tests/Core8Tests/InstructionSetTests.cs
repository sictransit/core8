using Core8.Extensions;
using Core8.Tests.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;

namespace Core8.Tests
{
    [TestClass]
    public class InstructionSetTests : PDPTestsBase
    {
        [TestMethod]
        public void TestDecode()
        {
            for (var i = 0; i <= 7777.ToDecimal(); i++)
            {
                var instruction = PDP.CPU.InstructionSet.Decode(i);

                instruction.Load(0200.ToDecimal(), i);

                Assert.IsNotNull(instruction);

                Log.Information(instruction.ToString());
            }
        }
    }
}