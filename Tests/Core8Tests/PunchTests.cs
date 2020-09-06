using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net.Http;

namespace Core8.Tests
{
    [TestClass]
    public class PunchTests
    {
        [TestMethod]
        public void TestPunch()
        {
            var label = "Punched paper tape as Scalable Vector Graphics (SVG), a beautiful anachronism! Today, folded for your convenience.";

            var url = @"https://github.com/PontusPih/TINT8/releases/download/v0.1.0-alpha/tint.bin";

            var data = new HttpClient().GetByteArrayAsync(url).Result.ToArray();

            //byte[] data = Encoding.ASCII.GetBytes(label).Concat(Enumerable.Range(0,256).Select(x=>(byte)x)).ToArray();

            var punch = new SVGPunch();

            var svg = punch.Punch(data, url);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(svg));

            Assert.IsTrue(svg.StartsWith("<svg width=\"7900\" height=\"1000\" xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns=\"http://www.w3.org/2000/svg\">"));
        }

        [TestMethod]
        public void TestReader()
        {
            var url = @"https://github.com/PontusPih/TINT8/releases/download/v0.1.0-alpha/tint.bin";

            var d0 = new HttpClient().GetByteArrayAsync(url).Result.ToArray();

            var punch = new SVGPunch();

            var svg = punch.Punch(d0, url);

            var reader = new SVGReader();

            var d1 = reader.Read(svg.ToString()).ToArray();

            Assert.IsNotNull(d1);
            Assert.IsTrue(d1.Any());

            for (int i = 0; i < d0.Length; i++)
            {
                Assert.AreEqual(d0[i], d1[i]);
            }
        }
    }

}
