using Core8.Tests.MAINDEC.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Core8.Tests.MAINDEC
{
    [TestClass]
    public class RandomJMPTest : MAINDECTestsBase
    {
        protected override string TapeName => @"MAINDEC/tapes/MAINDEC-8E-D0HC-PB.bin";

        protected override IEnumerable<string> ExpectedOutput => new[] { "\r\nHC" };

        protected override IEnumerable<string> UnexpectedOutput => new[] { "Z =" };

        [TestMethod]
        public override void Start()
        {
            PDP.Load8(0200);

            PDP.Toggle8(0000);

            PDP.Clear();

            Assert.IsTrue(StartAndWaitForCompletion());
        }
    }
}
