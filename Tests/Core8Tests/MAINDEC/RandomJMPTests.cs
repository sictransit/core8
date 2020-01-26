using Core8.Tests.MAINDEC.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Core8.Tests.MAINDEC
{
    [TestClass]
    public class RandomJMPTests : MAINDECTestsBase
    {
        protected override string TapeName => @"MAINDEC/tapes/MAINDEC-8E-D0HC-PB.bin";

        protected override string[] ExpectedOutput => new[] { "HC\r\nHC\r\nHC\r\nHC\r\nHC" };

        protected override string[] UnexpectedOutput => new[] { "Z =" };

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
