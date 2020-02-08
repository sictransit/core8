using Core8.Tests.MAINDEC.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Core8.Tests.MAINDEC
{
    [TestClass]
    public class BasicJMPJMSTest : MAINDECTestsBase
    {
        protected override string TapeName => @"MAINDEC/tapes/MAINDEC-8E-D0IB-PB.bin";

        protected override string[] ExpectedOutput => new[] { "\u0007" };

        protected override TimeSpan MaxRunningTime => TimeSpan.FromSeconds(10);

        [TestMethod]
        public void RunTest()
        {
            PDP.Load8(0200);

            PDP.Toggle8(0000);

            PDP.Clear();

            StartAndWaitForCompletion();
        }
    }
}
