using Core8.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core8Tests
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
    }
}
