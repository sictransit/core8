using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;

namespace Core8.Tests.Abstract
{
    [TestClass]
    public abstract class InstructionTestsBase
    {
        protected PDP PDP { get; private set; }

        [TestInitialize]
        public void Initialize()
        {
            PDP = new PDP();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();
        }

    }
}
