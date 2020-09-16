using Core8.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Core;
using System.Diagnostics;

namespace Core8.Tests.Abstract
{
    [TestClass]
    public abstract class InstructionTestsBase
    {
        protected PDP PDP { get; private set; }

        public LoggingLevelSwitch LoggingLevel { get; protected set; }

        [TestInitialize]
        public void Initialize()
        {
            LoggingLevel = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Information);

            //File.Delete("test.log");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information, outputTemplate: "[{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("test.log", outputTemplate: "[{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .MinimumLevel.ControlledBy(LoggingLevel)
                .CreateLogger();

            PDP = new PDP();

            PDP.CPU.Debug(Debugger.IsAttached);
        }
    }
}
