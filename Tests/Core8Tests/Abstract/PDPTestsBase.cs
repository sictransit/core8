using Core8.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using Serilog.Core;

namespace Core8.Tests.Abstract;

[TestClass]
public abstract class PDPTestsBase
{
    protected PDP PDP { get; private set; }

    protected LoggingLevelSwitch LoggingLevel { get; set; }

    [TestInitialize]
    public void Initialize()
    {
        LoggingLevel = new LoggingLevelSwitch();

        //File.Delete("test.log");

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(Serilog.Events.LogEventLevel.Information, "[{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("test.log", outputTemplate: "[{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .MinimumLevel.ControlledBy(LoggingLevel)
            .CreateLogger();

        PDP = new PDP();
    }
}
