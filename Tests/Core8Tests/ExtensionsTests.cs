using Core8.Model.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Core8.Tests
{
    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        public void TestToDecimal()
        {
            Assert.AreEqual(0, 0.ToDecimal());
            Assert.AreEqual(4095, 7777.ToDecimal());
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestToDecimalThrows()
        {
            var _ = 8.ToDecimal();
        }

        [TestMethod]
        public void TestToOctal()
        {
            Assert.AreEqual(7777, 4095.ToOctal());
        }

        [TestMethod]
        public void TestToOctalString()
        {
            Assert.AreEqual("7777", 4095.ToOctalString());
        }
    }
}
