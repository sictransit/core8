using Core8.Tests.MAINDEC.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core8.Tests.MAINDEC
{
    [TestClass]
    public class InstructionTestPart2 : MAINDECTestsBase
    {
        protected override string TapeName => @"MAINDEC/tapes/MAINDEC-8E-D0BB-PB.bin";

        protected override string[] ExpectedOutput => new[] { "\u0007" };

        [TestMethod]
        public override void Start()
        {
            PDP.Load8(0200);

            StartAndWaitForCompletion();
        }
    }
}
