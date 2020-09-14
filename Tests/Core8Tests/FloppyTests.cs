﻿using Core8.Floppy.Declarations;
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

            AssertDoneFlagSet(floppy);

            floppy.Initialize();

            AssertDoneFlagSet(floppy);
        }

        [TestMethod]
        public void TestNOP()
        {
            var floppy = new FloppyDrive();

            AssertDoneFlagSet(floppy);

            floppy.LoadCommandRegister(Functions.NO_OPERATION);

            AssertDoneFlagSet(floppy);
        }


        [TestMethod]
        public void TestFillBuffer()
        {
            var floppy = new FloppyDrive();

            AssertDoneFlagSet(floppy);

            FillBuffer(floppy, new int[64]);
        }

        [TestMethod]
        public void TestEmptyBuffer()
        {
            var floppy = new FloppyDrive();

            AssertDoneFlagSet(floppy);

            var buffer = EmptyBuffer(floppy);

            Assert.IsNotNull(buffer);

            Assert.AreEqual(64, buffer.Length);
        }

        [TestMethod]
        public void TestBufferIntegrity()
        {
            var floppy = new FloppyDrive();

            AssertDoneFlagSet(floppy);

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
                Thread.Sleep(20);

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
            floppy.LoadCommandRegister(Functions.FILL_BUFFER);

            Assert.IsTrue(floppy.SkipTransferRequest());

            for (int i = 0; i < data.Length; i++)
            {
                floppy.TransferDataRegister(data[i]);

                floppy.Tick();

                if (i < data.Length - 1)
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

        private int[] EmptyBuffer(FloppyDrive floppy)
        {
            var buffer = new int[64];

            floppy.LoadCommandRegister(Functions.EMPTY_BUFFER);

            Assert.IsTrue(floppy.SkipTransferRequest());

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = floppy.TransferDataRegister(0);

                floppy.Tick();

                if (i < 63)
                {
                    Assert.IsTrue(floppy.SkipTransferRequest());
                }
                else
                {
                    Assert.IsFalse(floppy.SkipTransferRequest());
                }
            }

            AssertDoneFlagSet(floppy);

            return buffer;
        }

        [TestMethod]
        public void TestReadWriteSector()
        {
            var floppy = new FloppyDrive();

            AssertDoneFlagSet(floppy);

            floppy.Load(0);

            floppy.Initialize();

            AssertDoneFlagSet(floppy);

            var written = GenerateRandomBlock();

            FillBuffer(floppy, written);

            var track = 47;
            var sector = 11;

            floppy.LoadCommandRegister(Functions.WRITE_SECTOR);

            Assert.IsTrue(floppy.SkipTransferRequest());

            floppy.TransferDataRegister(sector);

            Assert.IsTrue(floppy.SkipTransferRequest());

            floppy.TransferDataRegister(track);

            AssertDoneFlagSet(floppy);

            floppy.LoadCommandRegister(Functions.READ_SECTOR);

            Assert.IsTrue(floppy.SkipTransferRequest());

            floppy.TransferDataRegister(sector);

            Assert.IsTrue(floppy.SkipTransferRequest());

            floppy.TransferDataRegister(track);

            AssertDoneFlagSet(floppy);

            var read = EmptyBuffer(floppy);

            for (int i = 0; i < written.Length; i++)
            {
                Assert.AreEqual(written[i], read[i]);
            }
        }
    }
}
