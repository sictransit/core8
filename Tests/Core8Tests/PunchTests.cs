using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Core8.Tests
{
    [TestClass]
    public class PunchTests
    {
        [TestMethod]
        public void TestPunch()
        {
            var label = "Punched paper tape as Scalable Vector Graphics (SVG), a beautiful anachronism!";

            byte[] data = Encoding.ASCII.GetBytes(label);

            var svg = SVGPunch.Punch(data, label);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(svg));

            Assert.IsTrue(svg.StartsWith("<svg width=\"7900\" height=\"1000\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\">"));
        }
    }

}
