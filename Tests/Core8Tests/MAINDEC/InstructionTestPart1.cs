using Core8.Model.Extensions;
using Core8.Tests.MAINDEC.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Core8.Tests
{
    [TestClass]
    public class InstructionTestPart1 : MAINDECTestsBase
    {
        protected override string TapeName => @"MAINDEC/tapes/MAINDEC-8E-D0AB-PB.bin";

        protected override string[] ExpectedOutput => new[] { ((char)7).ToString() };

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(10);

        [TestMethod]
        public void RunTest()
        {
            PDP.Load8(0200);

            PDP.Toggle8(7777);

            PDP.Start();

            Assert.AreEqual(0147.ToDecimal(), PDP.Registers.IF_PC.Address);

            StartAndWaitForCompletion();
        }
    }
}
