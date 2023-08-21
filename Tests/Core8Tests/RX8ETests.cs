using Core8.Model.Interfaces;
using Core8.Peripherals.RX8E;
using Core8.Peripherals.RX8E.Declarations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Core8.Tests;

[TestClass]
public class RX8ETests
{
    [TestMethod]
    public void TestInitialize()
    {
        FloppyDrive floppy = new();

        AssertDoneFlagSet(floppy);

        floppy.Initialize();

        AssertDoneFlagSet(floppy);
    }

    [TestMethod]
    public void TestNOP()
    {
        FloppyDrive floppy = new();

        AssertDoneFlagSet(floppy);

        floppy.LoadCommandRegister(Functions.NO_OPERATION);

        AssertDoneFlagSet(floppy);
    }


    [TestMethod]
    public void TestFillBuffer()
    {
        FloppyDrive floppy = new();

        AssertDoneFlagSet(floppy);

        FillBuffer(floppy, new int[64]);
    }

    [TestMethod]
    public void TestEmptyBuffer()
    {
        FloppyDrive floppy = new();

        AssertDoneFlagSet(floppy);

        int[] buffer = EmptyBuffer(floppy);

        Assert.IsNotNull(buffer);

        Assert.AreEqual(64, buffer.Length);
    }

    [TestMethod]
    public void TestBufferIntegrity()
    {
        FloppyDrive floppy = new();

        AssertDoneFlagSet(floppy);

        int[] inData = GenerateRandomBlock();

        FillBuffer(floppy, inData);

        int[] outData = EmptyBuffer(floppy);

        for (int i = 0; i < inData.Length; i++)
        {
            Assert.AreEqual(inData[i], outData[i]);
        }
    }

    private static void AssertDoneFlagSet(IFloppyDrive floppy)
    {
        Stopwatch sw = new();
        sw.Start();

        bool done;

        do
        {
            floppy.Tick();

            done = floppy.SkipNotDone();

        } while (!done && sw.ElapsedMilliseconds < 2000);

        Assert.IsTrue(done);
    }

    private static int[] GenerateRandomBlock()
    {
        Random rnd = new();

        return Enumerable.Range(0, 64).Select(_ => rnd.Next(0, 4096)).ToArray();
    }

    private static void FillBuffer(IFloppyDrive floppy, int[] data)
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

    private static int[] EmptyBuffer(IFloppyDrive floppy)
    {
        int[] buffer = new int[64];

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
        FloppyDrive floppy = new();

        AssertDoneFlagSet(floppy);

        floppy.Load(0);

        floppy.Initialize();

        AssertDoneFlagSet(floppy);

        int[] written = GenerateRandomBlock();

        FillBuffer(floppy, written);

        int track = 47;
        int sector = 11;

        WriteSector(floppy, track, sector);

        ReadSector(floppy, track, sector);

        int[] read = EmptyBuffer(floppy);

        for (int i = 0; i < written.Length; i++)
        {
            Assert.AreEqual(written[i], read[i]);
        }
    }

    [TestMethod]
    public void TestLoadDiskImage()
    {
        FloppyDrive floppy = new();

        AssertDoneFlagSet(floppy);

        byte[] image = File.ReadAllBytes("disks/os8_rx.rx01");

        floppy.Load(0, image);

        floppy.Initialize();

        AssertDoneFlagSet(floppy);

        for (int track = DiskLayout.FIRST_TRACK; track <= DiskLayout.LAST_TRACK; track++)
        {
            for (int sector = DiskLayout.FIRST_SECTOR; sector <= DiskLayout.LAST_SECTOR; sector++)
            {
                ReadSector(floppy, track, sector);

                int[] buffer = EmptyBuffer(floppy);

                Assert.IsNotNull(buffer);

                Assert.AreEqual(64, buffer.Length);
            }
        }
    }

    private static void WriteSector(IFloppyDrive floppy, int track, int sector)
    {
        floppy.LoadCommandRegister(Functions.WRITE_SECTOR);

        SetSector(floppy, track, sector);
    }

    private static void ReadSector(IFloppyDrive floppy, int track, int sector)
    {
        floppy.LoadCommandRegister(Functions.READ_SECTOR);

        SetSector(floppy, track, sector);
    }

    private static void SetSector(IFloppyDrive floppy, int track, int sector)
    {
        Assert.IsTrue(floppy.SkipTransferRequest());

        floppy.TransferDataRegister(sector);

        Assert.IsTrue(floppy.SkipTransferRequest());

        floppy.TransferDataRegister(track);

        AssertDoneFlagSet(floppy);
    }
}
