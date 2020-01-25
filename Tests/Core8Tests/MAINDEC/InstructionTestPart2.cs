using Core8.Tests.MAINDEC.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Core8.Tests.MAINDEC
{
    [TestClass]
    public class InstructionTestPart2 : MAINDECTestsBase
    {
        protected override string TapeName => @"MAINDEC/tapes/MAINDEC-8E-D0BB-PB.bin";

        protected override string[] ExpectedOutput => new[] { ((char)7).ToString() };

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(10);

        [TestMethod]
        public void RunTest()
        {
            PDP.Load8(0200);

            StartAndWaitForCompletion();
        }
    }
}
