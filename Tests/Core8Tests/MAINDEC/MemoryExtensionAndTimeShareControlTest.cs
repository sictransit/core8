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

            PDP.Toggle8(4007);

            PDP.Clear();

            LoggingLevel.MinimumLevel = Serilog.Events.LogEventLevel.Information;

            StartAndWaitForCompletion();

            //PDP.SetBreakpoint8(2113);

            //LoggingLevel.MinimumLevel = Serilog.Events.LogEventLevel.Debug;

            //PDP.Continue();            

            //PDP.RemoveAllBreakpoints();

            //PDP.Continue();
        }
    }
}
