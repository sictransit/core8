using Core8.Tests.MAINDEC.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Core8.Tests.MAINDEC
{
    [TestClass]
    public class RandomJMPJMSTests : MAINDECTestsBase
    {
        protected override string TapeName => @"MAINDEC/tapes/MAINDEC-8E-D0JC-PB.bin";

        protected override string[] ExpectedOutput => new[] { "JC\r\nJC\r\nJC\r\nJC\r\nJC" };

        protected override string[] UnexpectedOutput => new[] { "F " };

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
