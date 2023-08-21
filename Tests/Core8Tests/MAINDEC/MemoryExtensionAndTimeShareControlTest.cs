using Core8.Tests.MAINDEC.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Core8.Tests.MAINDEC;

[TestClass]
public class MemoryExtensionAndTimeShareControlTest : MAINDECTestsBase
{
    protected override string TapeName => @"MAINDEC/tapes/MAINDEC-8E-D1HA-PB.bin";

    protected override IEnumerable<string> ExpectedOutput => new[] { "\u0007" };

    protected override TimeSpan MaxRunningTime => TimeSpan.FromSeconds(600);

    [TestMethod]
    public override void Start()
    {
        PDP.Load8(0200);

        PDP.Toggle8(0007);

        PDP.Clear();

        Assert.IsTrue(StartAndWaitForCompletion());
    }
}
