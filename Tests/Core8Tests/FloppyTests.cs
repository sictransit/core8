using Core8.Model.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace Core8.Tests
{
    [TestClass]
    public class FloppyTests
    {
        [TestMethod]
        public void TestFillBuffer()
        {
            var floppy = new FloppyDrive();

            floppy.LoadCommandRegister(0);

            Assert.IsTrue(floppy.SkipTransferRequest());

            for (int i = 0; i < 64; i++)
            {
                floppy.TransferDataRegister(0);

                if (i != 63)
                {
                    Assert.IsTrue(floppy.SkipTransferRequest());
                }
                else
                {
                    Assert.IsFalse(floppy.SkipTransferRequest());
                }
            }

            Thread.Sleep(floppy.CommandTime * 2);

            Assert.IsTrue(floppy.SkipNotDone());
        }

        [TestMethod]
        public void TestEmptyBuffer()
        {
            var floppy = new FloppyDrive();

            floppy.LoadCommandRegister(0b_010);

            Assert.IsTrue(floppy.SkipTransferRequest());

            for (int i = 0; i < 64; i++)
            {
                var acc = floppy.TransferDataRegister(0);

                Assert.AreEqual(0, acc);

                if (i != 63)
                {
                    Assert.IsTrue(floppy.SkipTransferRequest());
                }
                else
                {
                    Assert.IsFalse(floppy.SkipTransferRequest());
                }
            }

            Thread.Sleep(floppy.CommandTime * 2);

            Assert.IsTrue(floppy.SkipNotDone());
        }

        [TestMethod]
        public void TestBufferIntegrity()
        {
            var floppy = new FloppyDrive();

            var data = new int[64];
            var rnd = new Random();

            floppy.LoadCommandRegister(0);

            Assert.IsTrue(floppy.SkipTransferRequest());

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = rnd.Next(0, 4096);

                floppy.TransferDataRegister(data[i]);
            }

            Thread.Sleep(floppy.CommandTime * 2);

            Assert.IsTrue(floppy.SkipNotDone());

            floppy.LoadCommandRegister(0b_010);

            Assert.IsTrue(floppy.SkipTransferRequest());

            for (int i = 0; i < 64; i++)
            {
                var acc = floppy.TransferDataRegister(0);

                Assert.AreEqual(data[i], acc);
            }

            Thread.Sleep(floppy.CommandTime * 2);

            Assert.IsTrue(floppy.SkipNotDone());
        }
    }
}
