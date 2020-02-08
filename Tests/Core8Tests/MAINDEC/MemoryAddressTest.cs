using Core8.Tests.MAINDEC.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Core8.Tests.MAINDEC
{
    [TestClass]
    public class MemoryAddressTest : MAINDECTestsBase
    {
        protected override string TapeName => @"MAINDEC/tapes/MAINDEC-8E-D1EC-PB.bin";

        protected override string[] ExpectedOutput => new[] { "\r\nEC" };

        protected override TimeSpan MaxRunningTime => TimeSpan.FromSeconds(20);

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
