using Core8.Tests.MAINDEC.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Core8.Tests.MAINDEC
{
    [TestClass]
    public class RandomTADTest : MAINDECTestsBase
    {
        protected override string TapeName => @"MAINDEC/tapes/MAINDEC-8E-D0EB-PB.bin";

        protected override IEnumerable<string> ExpectedOutput => new[] { "\r\nT" };

        [TestMethod]
        public override void Start()
        {
            PDP.Load8(0200);

            PDP.Toggle8(0000);

            StartAndWaitForCompletion();
        }
    }
}
