using Core8.Tests.MAINDEC.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Core8.Tests.MAINDEC
{
    [TestClass]
    public class AdderTest : MAINDECTestsBase
    {
        protected override string TapeName => @"MAINDEC/tapes/MAINDEC-8E-D0CC-PB.bin";

        protected override IEnumerable<string> ExpectedOutput => new[] { "SIMAD", "SIMROT", "FCT", "RANDOM" };

        protected override TimeSpan MaxRunningTime => TimeSpan.FromSeconds(300);

        [TestMethod]
        public override void Start()
        {
            PDP.Load8(0200);

            PDP.Toggle8(0000);

            Assert.IsTrue(StartAndWaitForCompletion());
        }
    }
}
