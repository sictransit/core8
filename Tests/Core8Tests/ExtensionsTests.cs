using Core8.Extensions;
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
            Assert.AreEqual(0u, 0u.ToDecimal());
            Assert.AreEqual(4095u, 7777u.ToDecimal());
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestToDecimalThrows()
        {
            var _ = 8u.ToDecimal();
        }

        [TestMethod]
        public void TestToOctal()
        {
            Assert.AreEqual(7777u, 4095u.ToOctal());
        }

        [TestMethod]
        public void TestToOctalString()
        {
            Assert.AreEqual("7777", 4095u.ToOctalString());
        }
    }
}
