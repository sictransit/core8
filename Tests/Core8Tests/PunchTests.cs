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
            byte[] data = (new HttpClient()).GetByteArrayAsync(@"https://github.com/PontusPih/TINT8/releases/download/v0.1.0-alpha/tint.bin").Result;

            var svg = SvgPunch.Punch(data);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(svg));
        }
    }

}
