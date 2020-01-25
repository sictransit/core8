using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Core;

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

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.ControlledBy(LoggingLevel)
                .CreateLogger();

            PDP = new PDP();
        }
    }
}
