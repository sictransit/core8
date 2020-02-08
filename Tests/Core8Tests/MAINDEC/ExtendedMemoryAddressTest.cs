using Core8.Tests.MAINDEC.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Core8.Tests.MAINDEC
{
    [TestClass]
    public class ExtendedMemoryAddressTest : MAINDECTestsBase
    {
        protected override string TapeName => @"MAINDEC/tapes/MAINDEC-8E-D1FB-PB.bin";

        protected override string[] UnexpectedOutput => new[] { "PR LOC   ADDR   GOOD  BAD  TEST" };

        protected override string[] ExpectedOutput => new[] { "5\r\n5" };

        [TestMethod]
        public void RunTest()
        {
            PDP.Load8(0200);

            PDP.Continue();

            Assert.IsTrue(PDP.Teleprinter.Printout.Contains("EA8-E EXT MEM ADDR TEST"));
            Assert.IsTrue(PDP.Teleprinter.Printout.Contains("SETUP SR & CONT"));

            PDP.Toggle8(0007);

            StartAndWaitForCompletion();
        }
    }
}
