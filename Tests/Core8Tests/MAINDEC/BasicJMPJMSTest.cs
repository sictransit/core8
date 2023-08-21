using Core8.Tests.MAINDEC.Abstract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Core8.Tests.MAINDEC;

[TestClass]
public class BasicJMPJMSTest : MAINDECTestsBase
{
    protected override string TapeName => @"MAINDEC/tapes/MAINDEC-8E-D0IB-PB.bin";

    protected override IEnumerable<string> ExpectedOutput => new[] { "\u0007" };

    [TestMethod]
    public override void Start()
    {
        PDP.Load8(0200);

        PDP.Toggle8(0000);

        PDP.Clear();

        Assert.IsTrue(StartAndWaitForCompletion());
    }
}
