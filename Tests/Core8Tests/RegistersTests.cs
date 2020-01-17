using Core8.Model;
using Core8.Tests.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;

namespace Core8.Tests
{
    [TestClass]
    public class RegistersTests : InstructionTestsBase
    {
        [TestMethod]
        public void TestSetAccumulatorPerformance()
        {
            var sw = new Stopwatch();
            var rnd = new Random();

            var values = Enumerable.Range(0, 10000000).Select(x => (uint)rnd.Next(0, 4096)).ToArray();

            for (int i = 0; i < 10; i++)
            {

                sw.Restart();

                foreach (var value in values)
                {
                    PDP.Registers.LINK_AC.SetAccumulator(value);
                    PDP.Registers.LINK_AC.SetLink(value & Masks.FLAG);
                }

                Trace.WriteLine(sw.ElapsedMilliseconds);
            }

            Assert.AreEqual(values.Last(), PDP.Registers.LINK_AC.Accumulator);
            Assert.AreEqual(values.Last() & Masks.FLAG, PDP.Registers.LINK_AC.Link);

        }

        [TestMethod]
        public void TestRotateAccumulatorPerformance()
        {
            var sw = new Stopwatch();
            var rnd = new Random();

            for (int i = 0; i < 10; i++)
            {
                PDP.Registers.LINK_AC.SetAccumulator((uint)rnd.Next(3, 4096));

                sw.Restart();

                for (int j = 0; j < 10000000; j++)
                {
                    if (j % 17 == 0)
                    {
                        PDP.Registers.LINK_AC.RAL();
                    }
                    else
                    {
                        PDP.Registers.LINK_AC.RAL();
                    }

                }

                Trace.WriteLine(sw.ElapsedMilliseconds);

            }

            Assert.IsTrue(PDP.Registers.LINK_AC.Accumulator != 0);
        }
    }
}
