using Core8.Peripherals.Teletype;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;

namespace Core8.Tests
{
    [TestClass]
    public class PunchTests
    {
        private const string LABEL = "Punched paper tape as Scalable Vector Graphics (SVG), a beautiful anachronism! Today, folded for your convenience.";

        private static byte[] TestData => Encoding.ASCII.GetBytes(LABEL).Concat(Enumerable.Range(0, 256).Select(x => (byte)x)).ToArray();

        [TestMethod]
        public void TestPunch()
        {
            var punch = new SVGPunch();

            var svg = punch.Punch(TestData, LABEL);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(svg));

            Assert.IsTrue(svg.StartsWith("<svg"));
            Assert.IsTrue(svg.EndsWith("</svg>"));
        }

        [TestMethod]
        public void TestReader()
        {
            var punch = new SVGPunch();

            var svg = punch.Punch(TestData, LABEL);

            var data = SVGReader.Read(svg).ToArray();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Any());

            for (var i = 0; i < TestData.Length; i++)
            {
                Assert.AreEqual(TestData[i], data[i]);
            }
        }

        [TestMethod]
        public void TestColor()
        {
            var data = Enumerable.Range(0, 255).Select(x => (byte)x).ToArray();

            var defaultPunch = new SVGPunch(0, true);
            var defaultPaper = defaultPunch.Punch(data);
            Assert.IsTrue(defaultPaper.Contains(SVGPunch.DefaultPaperColor));

            const string blue = "#CCE5FF";
            var bluePunch = new SVGPunch(0, true, blue);
            var bluePaper = bluePunch.Punch(data);
            Assert.IsTrue(bluePaper.Contains(blue));

            const string pink = "#FFCCCC";
            var pinkPunch = new SVGPunch(0, true, pink);
            var pinkPaper = pinkPunch.Punch(data);
            Assert.IsTrue(pinkPaper.Contains(pink));
        }
    }
}
