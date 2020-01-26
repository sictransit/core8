using Core8.Tests.MAINDEC.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Core8.Tests.MAINDEC
{
    [TestClass]
    public class RandomTADTests : MAINDECTestsBase
    {
        protected override string TapeName => @"MAINDEC/tapes/MAINDEC-8E-D0EB-PB.bin";

        protected override string[] ExpectedOutput => new[] { "T\r\nT\r\nT\r\nT\r\nT\r\nT\r\nT\r\nT\r\nT\r\nT" };

        protected override TimeSpan MaxRunningTime => TimeSpan.FromSeconds(30);

        [TestMethod]
        public void RunTest()
        {
            PDP.Load8(0200);

            PDP.Toggle8(0000);

            StartAndWaitForCompletion();
        }
    }
}
