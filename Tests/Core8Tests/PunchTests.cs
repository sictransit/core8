using Core8.Peripherals.Teletype;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
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

            Assert.IsTrue(svg.StartsWith("<?xml"));
            Assert.IsTrue(svg.Contains("<svg"));
            Assert.IsTrue(svg.EndsWith("</svg>"));
        }

        [TestMethod]
        public void TestReader()
        {
            var punch = new SVGPunch(new PunchSettings(Color.WhiteSmoke, 0, false, 0));

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

            var defaultPunch = new SVGPunch();
            var defaultPaper = defaultPunch.Punch(data, "default");
            Assert.IsTrue(defaultPaper.Contains(ColorTranslator.ToHtml(Color.LightYellow)));

            var blue = Color.LightBlue;
            var bluePunch = new SVGPunch(new PunchSettings(blue));
            var bluePaper = bluePunch.Punch(data, "blue");
            Assert.IsTrue(bluePaper.Contains(ColorTranslator.ToHtml(blue)));

            var pink = Color.LightPink;
            var pinkPunch = new SVGPunch(new PunchSettings(pink));
            var pinkPaper = pinkPunch.Punch(data, "pink");
            Assert.IsTrue(pinkPaper.Contains(ColorTranslator.ToHtml(pink)));
        }
    }
}
