using Core8.Extensions;
using Core8.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Core8.Tests;

[TestClass]
public class RegistryTests
{
    [TestMethod]
    public void TestAC()
    {
        Registry registry = new();

        for (int i = 0; i <= 7777.ToDecimal(); i++)
        {
            registry.AC.SetAccumulator(i);

            Assert.AreEqual(i, registry.AC.Accumulator);
        }

        registry.AC.SetAccumulator(10000.ToDecimal());

        Assert.AreEqual(0, registry.AC.Accumulator);
        Assert.AreEqual(0, registry.AC.Link);

        registry.AC.SetLink(1);
        Assert.AreEqual(1, registry.AC.Link);
    }

    [TestMethod]
    public void TestToString()
    {
        Registry registry = new();

        string s = registry.ToString();

        Assert.IsFalse(string.IsNullOrWhiteSpace(s));

        Trace.WriteLine(s);
    }
}