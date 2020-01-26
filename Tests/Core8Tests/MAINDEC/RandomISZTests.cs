using Core8.Tests.MAINDEC.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Core8.Tests.MAINDEC
{
    [TestClass]
    public class RandomISZTests : MAINDECTestsBase
    {
        protected override string TapeName => @"MAINDEC/tapes/MAINDEC-8E-D0FC-PB.bin";

        protected override string[] ExpectedOutput => new[] { "\r\nFC\u007f\r\nFC\u007f\r\nFC\u007f\r\nFC\u007f\r\nFC\u007f" };

        [TestMethod]
        public void RunTest()
        {
            PDP.Load8(0200);

            PDP.Toggle8(0000);

            StartAndWaitForCompletion();
        }
    }
}
