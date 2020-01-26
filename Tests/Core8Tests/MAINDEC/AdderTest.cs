using Core8.Tests.MAINDEC.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Core8.Tests.MAINDEC
{
    [TestClass]
    public class AdderTest : MAINDECTestsBase
    {
        protected override string TapeName => @"MAINDEC/tapes/MAINDEC-8E-D0CC-PB.bin";

        protected override string[] ExpectedOutput => new[] { "SIMAD", "SIMROT", "FCT", "RANDOM" };

        protected override TimeSpan MaxRunningTime => TimeSpan.FromSeconds(120);

        [TestMethod]
        public void RunTest()
        {
            PDP.Load8(0200);

            PDP.Toggle8(0000);

            StartAndWaitForCompletion();
        }
    }
}
