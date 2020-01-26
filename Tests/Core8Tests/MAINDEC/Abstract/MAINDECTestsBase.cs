using Core8.Tests.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Core8.Tests.MAINDEC.Abstract
{
    [TestClass]
    public abstract class MAINDECTestsBase : InstructionTestsBase
    {
        protected abstract string TapeName { get; }

        protected virtual string[] ExpectedOutput => new string[] { };

        protected virtual string[] UnexpectedOutput => new string[] { };

        protected virtual TimeSpan MaxRunningTime => TimeSpan.FromSeconds(10); 

        protected virtual bool ExpectHLT => false;

        [TestInitialize]
        public void LoadTape()
        {
            PDP.Clear();

            PDP.LoadPaperTape(File.ReadAllBytes(TapeName));

            PDP.Clear();
        }

        protected void StartAndWaitForCompletion()
        {
            PDP.Start(waitForHalt: false);

            var sw = new Stopwatch();
            sw.Start();

            var done = false;
            var failed = false;
            var timeout = false;

            while (!timeout && !done && !failed && PDP.Running)
            {
                done = ExpectedOutput.Any() && ExpectedOutput.All(x => PDP.Teleprinter.Printout.Contains(x));

                failed = UnexpectedOutput.Any(x => PDP.Teleprinter.Printout.Contains(x));

                timeout = sw.Elapsed > MaxRunningTime;

                Thread.Sleep(200);
            }

            if (!ExpectHLT)
            {
                Assert.IsTrue(PDP.Running);
            }

            Assert.IsFalse(failed);

            if (!ExpectedOutput.Any())
            {
                Assert.IsFalse(done);
                Assert.IsTrue(timeout);
            }
            else
            {
                Assert.IsTrue(done);
                Assert.IsFalse(timeout);
            }
        }
    }
}
