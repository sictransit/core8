using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;

namespace Core8.Tests
{
    [TestClass]
    public class PunchTests
    {
        private const string Label = "Punched paper tape as Scalable Vector Graphics (SVG), a beautiful anachronism! Today, folded for your convenience.";

        private byte[] TestData => Encoding.ASCII.GetBytes(Label).Concat(Enumerable.Range(0, 256).Select(x => (byte)x)).ToArray();

        [TestMethod]
        public void TestPunch()
        {
            var punch = new SVGPunch();

            var svg = punch.Punch(TestData, Label);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(svg));

            Assert.IsTrue(svg.StartsWith("<svg"));
            Assert.IsTrue(svg.EndsWith("</svg>"));
        }

        [TestMethod]
        public void TestReader()
        {
            var punch = new SVGPunch();

            var svg = punch.Punch(TestData, Label);

            var reader = new SVGReader();

            var data = reader.Read(svg.ToString()).ToArray();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Any());

            for (int i = 0; i < TestData.Length; i++)
            {
                Assert.AreEqual(TestData[i], data[i]);
            }
        }
    }

}
