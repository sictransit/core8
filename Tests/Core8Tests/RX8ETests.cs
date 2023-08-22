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
        RX8EController controller = new();

        AssertDoneFlagSet(controller);

        controller.Initialize();

        AssertDoneFlagSet(controller);
    }

    [TestMethod]
    public void TestNOP()
    {
        RX8EController controller = new();

        AssertDoneFlagSet(controller);

        controller.LoadCommandRegister(Functions.NO_OPERATION);

        AssertDoneFlagSet(controller);
    }


    [TestMethod]
    public void TestFillBuffer()
    {
        RX8EController controller = new();

        AssertDoneFlagSet(controller);

        FillBuffer(controller, new int[64]);
    }

    [TestMethod]
    public void TestEmptyBuffer()
    {
        RX8EController controller = new();

        AssertDoneFlagSet(controller);

        var buffer = EmptyBuffer(controller);

        Assert.IsNotNull(buffer);

        Assert.AreEqual(64, buffer.Length);
    }

    [TestMethod]
    public void TestBufferIntegrity()
    {
        RX8EController controller = new();

        AssertDoneFlagSet(controller);

        var inData = GenerateRandomBlock();

        FillBuffer(controller, inData);

        var outData = EmptyBuffer(controller);

        for (var i = 0; i < inData.Length; i++)
        {
            Assert.AreEqual(inData[i], outData[i]);
        }
    }

    private static void AssertDoneFlagSet(IRX8E controller)
    {
        Stopwatch sw = new();
        sw.Start();

        bool done;

        do
        {
            controller.Tick();

            done = controller.SkipNotDone();

        } while (!done && sw.ElapsedMilliseconds < 2000);

        Assert.IsTrue(done);
    }

    private static int[] GenerateRandomBlock()
    {
        Random rnd = new();

        return Enumerable.Range(0, 64).Select(_ => rnd.Next(0, 4096)).ToArray();
    }

    private static void FillBuffer(IRX8E controller, int[] data)
    {
        controller.LoadCommandRegister(Functions.FILL_BUFFER);

        Assert.IsTrue(controller.SkipTransferRequest());

        for (var i = 0; i < data.Length; i++)
        {
            controller.TransferDataRegister(data[i]);

            controller.Tick();

            if (i < data.Length - 1)
            {
                Assert.IsTrue(controller.SkipTransferRequest());
            }
            else
            {
                Assert.IsFalse(controller.SkipTransferRequest());
            }
        }

        AssertDoneFlagSet(controller);
    }

    private static int[] EmptyBuffer(IRX8E controller)
    {
        var buffer = new int[64];

        controller.LoadCommandRegister(Functions.EMPTY_BUFFER);

        Assert.IsTrue(controller.SkipTransferRequest());

        for (var i = 0; i < buffer.Length; i++)
        {
            buffer[i] = controller.TransferDataRegister(0);

            controller.Tick();

            if (i < 63)
            {
                Assert.IsTrue(controller.SkipTransferRequest());
            }
            else
            {
                Assert.IsFalse(controller.SkipTransferRequest());
            }
        }

        AssertDoneFlagSet(controller);

        return buffer;
    }

    [TestMethod]
    public void TestReadWriteSector()
    {
        RX8EController controller = new();

        AssertDoneFlagSet(controller);

        controller.Load(0);

        controller.Initialize();

        AssertDoneFlagSet(controller);

        var written = GenerateRandomBlock();

        FillBuffer(controller, written);

        var track = 47;
        var sector = 11;

        WriteSector(controller, track, sector);

        ReadSector(controller, track, sector);

        var read = EmptyBuffer(controller);

        for (var i = 0; i < written.Length; i++)
        {
            Assert.AreEqual(written[i], read[i]);
        }
    }

    [TestMethod]
    public void TestLoadDiskImage()
    {
        RX8EController controller = new();

        AssertDoneFlagSet(controller);

        var image = File.ReadAllBytes("disks/os8_rx.rx01");

        controller.Load(0, image);

        controller.Initialize();

        AssertDoneFlagSet(controller);

        for (var track = DiskLayout.FIRST_TRACK; track <= DiskLayout.LAST_TRACK; track++)
        {
            for (var sector = DiskLayout.FIRST_SECTOR; sector <= DiskLayout.LAST_SECTOR; sector++)
            {
                ReadSector(controller, track, sector);

                var buffer = EmptyBuffer(controller);

                Assert.IsNotNull(buffer);

                Assert.AreEqual(64, buffer.Length);
            }
        }
    }

    private static void WriteSector(IRX8E controller, int track, int sector)
    {
        controller.LoadCommandRegister(Functions.WRITE_SECTOR);

        SetSector(controller, track, sector);
    }

    private static void ReadSector(IRX8E controller, int track, int sector)
    {
        controller.LoadCommandRegister(Functions.READ_SECTOR);

        SetSector(controller, track, sector);
    }

    private static void SetSector(IRX8E controller, int track, int sector)
    {
        Assert.IsTrue(controller.SkipTransferRequest());

        controller.TransferDataRegister(sector);

        Assert.IsTrue(controller.SkipTransferRequest());

        controller.TransferDataRegister(track);

        AssertDoneFlagSet(controller);
    }
}
