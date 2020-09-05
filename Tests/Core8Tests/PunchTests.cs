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
            //byte[] data = (new HttpClient()).GetByteArrayAsync(@"https://github.com/PontusPih/TINT8/releases/download/v0.1.0-alpha/tint.bin").Result;

            var label = "Punched paper tape as Scalable Vector Graphics (SVG), a beautiful anachronism!";
            byte[] data = Encoding.ASCII.GetBytes(label);

            var svg = SVGPunch.Punch(data, label);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(svg));
        }
    }

}
