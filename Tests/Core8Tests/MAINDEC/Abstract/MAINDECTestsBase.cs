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

        protected abstract string[] ExpectedOutput { get; }

        protected abstract TimeSpan Timeout { get; }

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

            while (sw.Elapsed < Timeout && !done)
            {
                done = ExpectedOutput.All(x => PDP.Teleprinter.Printout.Contains(x));

                Thread.Sleep(200);
            }

            Assert.IsTrue(done);
        }
    }
}
