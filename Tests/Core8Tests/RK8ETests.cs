﻿using Core8.Peripherals.RK8E;
using Core8.Tests.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Core8.Tests;

[TestClass]
public class RK8ETests : PDPTestsBase
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void TestLoadRXDiskImageWithFailure()
    {
        RK8EController disk = new(PDP.CPU.Memory);

        var image = File.ReadAllBytes("disks/os8_rx.rx01");

        disk.Load(0, image);

        Assert.Fail("didn't get the expected exception");
    }

    [TestMethod]
    public void TestLoadRKDiskImage()
    {
        RK8EController disk = new(PDP.CPU.Memory);

        var image = File.ReadAllBytes("disks/advent.rk05");

        for (var unit = 0; unit < 4; unit++)
        {
            disk.Load(unit, image);
        }

        Assert.IsTrue(true);
    }
}