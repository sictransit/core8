using Core8.Model.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
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

            LoadCommand(floppy, FloppyDrive.FILL_BUFFER);

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

            Thread.Sleep(FloppyDrive.CommandTime * 2);

            Assert.IsTrue(floppy.SkipNotDone());
        }

        [TestMethod]
        public void TestEmptyBuffer()
        {
            var floppy = new FloppyDrive();

            LoadCommand(floppy, FloppyDrive.EMPTY_BUFFER);

            for (int i = 0; i < 64; i++)
            {
                var acc = floppy.TransferDataRegister(0);

                Assert.AreEqual(0, acc);

                Assert.IsTrue(floppy.SkipTransferRequest());
            }

            var _ = floppy.TransferDataRegister(0);

            Assert.IsFalse(floppy.SkipTransferRequest());

            Thread.Sleep(FloppyDrive.CommandTime * 2);

            Assert.IsTrue(floppy.SkipNotDone());
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

        private int[] GenerateRandomBlock()
        {
            var rnd = new Random();

            return Enumerable.Range(0, 64).Select(x => rnd.Next(0, 4096)).ToArray();
        }

        private void LoadCommand(IFloppyDrive floppy, int command)
        {
            floppy.LoadCommandRegister(command);

            Assert.IsTrue(floppy.SkipTransferRequest());
        }

        private void FillBuffer(FloppyDrive floppy, int[] data)
        {
            LoadCommand(floppy, FloppyDrive.FILL_BUFFER);

            for (int i = 0; i < data.Length; i++)
            {
                floppy.TransferDataRegister(data[i]);
            }

            Thread.Sleep(FloppyDrive.CommandTime * 2);

            Assert.IsTrue(floppy.SkipNotDone());
        }

        private int[] EmptyBuffer(FloppyDrive floppy)
        {
            LoadCommand(floppy, FloppyDrive.EMPTY_BUFFER);

            var data = new int[64];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = floppy.TransferDataRegister(0);
            }

            floppy.TransferDataRegister(0);

            Thread.Sleep(FloppyDrive.CommandTime * 2);

            Assert.IsTrue(floppy.SkipNotDone());

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

            Thread.Sleep(FloppyDrive.InitializeTime * 2);

            Assert.IsTrue(floppy.SkipNotDone());

            var written = GenerateRandomBlock();

            FillBuffer(floppy, written);

            var track = 47;
            var sector = 11;

            LoadCommand(floppy, FloppyDrive.WRITE_SECTOR);

            floppy.TransferDataRegister(sector);

            Assert.IsTrue(floppy.SkipTransferRequest());

            floppy.TransferDataRegister(track);

            Assert.IsFalse(floppy.SkipTransferRequest());
            Assert.IsFalse(floppy.SkipNotDone());

            Thread.Sleep(FloppyDrive.AverageAccessTime * 2);

            Assert.IsTrue(floppy.SkipNotDone());

            LoadCommand(floppy, FloppyDrive.READ_SECTOR);

            floppy.TransferDataRegister(sector);

            Assert.IsTrue(floppy.SkipTransferRequest());

            floppy.TransferDataRegister(track);

            Assert.IsFalse(floppy.SkipTransferRequest());
            Assert.IsFalse(floppy.SkipNotDone());

            Thread.Sleep(FloppyDrive.AverageAccessTime * 2);

            Assert.IsTrue(floppy.SkipNotDone());

            var read = EmptyBuffer(floppy);

            for (int i = 0; i < written.Length; i++)
            {
                Assert.AreEqual(written[i], read[i]);
            }
        }
    }
}
