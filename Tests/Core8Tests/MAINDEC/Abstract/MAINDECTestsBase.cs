using Core8.Tests.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Core8.Tests.MAINDEC.Abstract
{
    [TestClass]
    public abstract class MAINDECTestsBase : PDPTestsBase
    {
        protected abstract string TapeName { get; }

        protected virtual IEnumerable<string> ExpectedOutput => Array.Empty<string>();

        protected virtual IEnumerable<string> UnexpectedOutput => Array.Empty<string>();

        protected virtual TimeSpan MaxRunningTime => TimeSpan.FromSeconds(60);

        [TestInitialize]
        public void LoadTape()
        {
            PDP.Clear();

            PDP.LoadPaperTape(File.ReadAllBytes(TapeName));

            PDP.Clear();
        }

        public abstract void Start();

        protected bool StartAndWaitForCompletion()
        {
            var result = true;

            PDP.Continue(false);

            var sw = new Stopwatch();
            sw.Start();

            var done = false;
            var failed = false;
            var timeout = false;

            while (!timeout && !done && !failed && PDP.Running)
            {
                done = ExpectedOutput.Any() && ExpectedOutput.All(x => PDP.CPU.PrinterPunch.Printout.Contains(x));

                failed = UnexpectedOutput.Any(x => PDP.CPU.PrinterPunch.Printout.Contains(x));

                timeout = !Debugger.IsAttached && sw.Elapsed > MaxRunningTime;

                Thread.Sleep(50);
            }

            PDP.Halt();

            Assert.IsFalse(failed);

            if (ExpectedOutput.Any())
            {
                result &= done;
                result &= !timeout;
            }
            else
            {
                result &= !done;
                result &= timeout;
            }

            PDP.Halt();

            return result;
        }
    }
}
