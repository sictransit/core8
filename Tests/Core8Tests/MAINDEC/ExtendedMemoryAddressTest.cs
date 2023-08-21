using Core8.Tests.MAINDEC.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Core8.Tests.MAINDEC
{
    [TestClass]
    public class ExtendedMemoryAddressTest : MAINDECTestsBase
    {
        protected override string TapeName => @"MAINDEC/tapes/MAINDEC-8E-D1FB-PB.bin";

        protected override IEnumerable<string> UnexpectedOutput => new[] { "PR LOC   ADDR   GOOD  BAD  TEST" };

        protected override IEnumerable<string> ExpectedOutput => new[] { "\r\n5" };

        protected override TimeSpan MaxRunningTime => TimeSpan.FromSeconds(600);

        [TestMethod]
        public override void Start()
        {
            PDP.Load8(0200);

            PDP.Continue();

            Assert.IsTrue(PDP.CPU.PrinterPunch.Printout.Contains("EA8-E EXT MEM ADDR TEST"));
            Assert.IsTrue(PDP.CPU.PrinterPunch.Printout.Contains("SETUP SR & CONT"));

            PDP.Toggle8(0007);

            Assert.IsTrue(StartAndWaitForCompletion());
        }
    }
}
