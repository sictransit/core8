using Core8.Tests.MAINDEC.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Core8.Tests.MAINDEC
{
    [TestClass]
    public class MemoryExtensionAndTimeShareControlTest : MAINDECTestsBase
    {
        protected override string TapeName => @"MAINDEC/tapes/MAINDEC-8E-D1HA-PB.bin";

        protected override string[] ExpectedOutput => new[] { "\u0007" };

        protected override TimeSpan MaxRunningTime => TimeSpan.FromSeconds(60);

        [TestMethod]
        public void RunTest()
        {
            PDP.Load8(0200);

            PDP.Toggle8(0007);

            PDP.Clear();

            //LoggingLevel.MinimumLevel = Serilog.Events.LogEventLevel.Debug;

            //StartAndWaitForCompletion();

            PDP.SetBreakpoint8(2113);

            PDP.Continue();

            LoggingLevel.MinimumLevel = Serilog.Events.LogEventLevel.Verbose;

            PDP.RemoveAllBreakpoints();

            PDP.Continue();
        }
    }
}
