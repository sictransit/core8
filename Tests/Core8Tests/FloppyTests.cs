using Core8.Floppy.Declarations;
using Core8.Model.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Core8.Tests
{
    [TestClass]
    public class FloppyTests
    {
        [TestMethod]
        public void TestInitialize()
        {
            var floppy = new FloppyDrive();

            floppy.Initialize();

            AssertDoneFlagSet(floppy);
        }

        [TestMethod]
        public void TestFillBuffer()
        {
            var floppy = new FloppyDrive();

            LoadCommand(floppy, Functions.FILL_BUFFER);

            for (int i = 0; i < 64; i++)
            {
                floppy.TransferDataRegister(0);

                floppy.Tick();

                if (i != 63)
                {
                    Assert.IsTrue(floppy.SkipTransferRequest());
                }
                else
                {
                    Assert.IsFalse(floppy.SkipTransferRequest());
                }
            }

            AssertDoneFlagSet(floppy);
        }

        [TestMethod]
        public void TestEmptyBuffer()
        {
            var floppy = new FloppyDrive();

            LoadCommand(floppy, Functions.EMPTY_BUFFER);

            for (int i = 0; i < 64; i++)
            {
                var acc = floppy.TransferDataRegister(0);

                Assert.AreEqual(0, acc);

                Assert.IsTrue(floppy.SkipTransferRequest());
            }

            var _ = floppy.TransferDataRegister(0);

            Assert.IsFalse(floppy.SkipTransferRequest());

            AssertDoneFlagSet(floppy);
        }

        [TestMethod]
        public void TestBufferIntegrity()
        {
            var floppy = new FloppyDrive();

            var inData = GenerateRandomBlock();

            FillBuffer(floppy, inData);

            var outData = EmptyBuffer(floppy);

            for (int i = 0; i < inData.Length; i++)
            {
                Assert.AreEqual(inData[i], outData[i]);
            }
        }

        private void AssertDoneFlagSet(IFloppyDrive floppy, int timeout = 10000)
        {
            var sw = new Stopwatch();
            sw.Start();

            bool done;

            do
            {
                Thread.Sleep(100);

                floppy.Tick();

                done = floppy.SkipNotDone();

            } while (!done && sw.ElapsedMilliseconds < timeout);

            Assert.IsTrue(done);
        }

        private int[] GenerateRandomBlock()
        {
            var rnd = new Random();

            return Enumerable.Range(0, 64).Select(x => rnd.Next(0, 4096)).ToArray();
        }

        private void LoadCommand(IFloppyDrive floppy, int command)
        {
            floppy.LoadCommandRegister(command);

            floppy.Tick();

            Assert.IsTrue(floppy.SkipTransferRequest());
        }

        private void FillBuffer(FloppyDrive floppy, int[] data)
        {
            LoadCommand(floppy, Functions.FILL_BUFFER);

            for (int i = 0; i < data.Length; i++)
            {
                floppy.TransferDataRegister(data[i]);
            }

            AssertDoneFlagSet(floppy);
        }

        private int[] EmptyBuffer(FloppyDrive floppy)
        {
            LoadCommand(floppy, Functions.EMPTY_BUFFER);

            var data = new int[64];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = floppy.TransferDataRegister(0);
            }

            floppy.TransferDataRegister(0);

            AssertDoneFlagSet(floppy);

            return data;
        }

        [TestMethod]
        public void TestReadWriteSector()
        {
            var floppy = new FloppyDrive();

            var disk = new byte[77 * 26 * 128];

            Assert.IsFalse(floppy.SkipNotDone());

            floppy.Load(0, disk);
            floppy.Initialize();

            AssertDoneFlagSet(floppy);

            var written = GenerateRandomBlock();

            FillBuffer(floppy, written);

            var track = 47;
            var sector = 11;

            LoadCommand(floppy, Functions.WRITE_SECTOR);

            floppy.TransferDataRegister(sector);

            Assert.IsTrue(floppy.SkipTransferRequest());

            floppy.TransferDataRegister(track);

            Assert.IsFalse(floppy.SkipTransferRequest());
            Assert.IsFalse(floppy.SkipNotDone());

            AssertDoneFlagSet(floppy);

            LoadCommand(floppy, Functions.READ_SECTOR);

            floppy.TransferDataRegister(sector);

            Assert.IsTrue(floppy.SkipTransferRequest());

            floppy.TransferDataRegister(track);

            Assert.IsFalse(floppy.SkipTransferRequest());
            Assert.IsFalse(floppy.SkipNotDone());

            AssertDoneFlagSet(floppy);

            var read = EmptyBuffer(floppy);

            for (int i = 0; i < written.Length; i++)
            {
                Assert.AreEqual(written[i], read[i]);
            }
        }
    }
}
