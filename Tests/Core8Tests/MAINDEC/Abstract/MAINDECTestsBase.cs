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

        protected virtual TimeSpan MaxRunningTime => TimeSpan.FromSeconds(15);

        protected virtual bool ExpectHLT => false;

        [TestInitialize]
        public void LoadTape()
        {
            PDP.Clear();

            PDP.LoadPaperTape(File.ReadAllBytes(TapeName));

            PDP.Clear();
        }

        public abstract void Start();

        protected void StartAndWaitForCompletion()
        {
            PDP.Continue(waitForHalt: false);

            var sw = new Stopwatch();
            sw.Start();

            var done = false;
            var failed = false;
            var timeout = false;

            while (!timeout && !done && !failed && PDP.Running)
            {
                done = ExpectedOutput.Any() && ExpectedOutput.All(x => PDP.CPU.Teletype.Printout.Contains(x));

                failed = UnexpectedOutput.Any(x => PDP.CPU.Teletype.Printout.Contains(x));

                timeout = !Debugger.IsAttached && (sw.Elapsed > MaxRunningTime);

                Thread.Sleep(50);
            }

            if (!ExpectHLT)
            {
                Assert.IsTrue(PDP.Running);
            }

            PDP.Halt();

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

            PDP.Halt();
        }
    }
}
