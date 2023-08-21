using Core8.Peripherals.Teletype;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Core8.Tests;

[TestClass]
public class PunchTests
{
    private const string LABEL = "Punched paper tape as Scalable Vector Graphics (SVG), a beautiful anachronism! Today, folded for your convenience.";

    private static byte[] TestData => Encoding.ASCII.GetBytes(LABEL).Concat(Enumerable.Range(0, 256).Select(x => (byte)x)).ToArray();

    [TestMethod]
    public void TestPunch()
    {
        SVGPunch punch = new();

        string svg = punch.Punch(TestData, LABEL);

        Assert.IsTrue(!string.IsNullOrWhiteSpace(svg));

        Assert.IsTrue(svg.StartsWith("<?xml"));
        Assert.IsTrue(svg.Contains("<svg"));
        Assert.IsTrue(svg.EndsWith("</svg>"));
    }

    [TestMethod]
    public void TestReader()
    {
        SVGPunch punch = new(new PunchSettings(Color.WhiteSmoke, 0, false, 0));

        string svg = punch.Punch(TestData, LABEL);

        byte[] data = SVGReader.Read(svg).ToArray();

        Assert.IsNotNull(data);
        Assert.IsTrue(data.Any());

        for (int i = 0; i < TestData.Length; i++)
        {
            Assert.AreEqual(TestData[i], data[i]);
        }
    }

    [TestMethod]
    public void TestColor()
    {
        byte[] data = Enumerable.Range(0, 255).Select(x => (byte)x).ToArray();

        SVGPunch defaultPunch = new();
        string defaultPaper = defaultPunch.Punch(data, "default");
        Assert.IsTrue(defaultPaper.Contains(ColorTranslator.ToHtml(Color.LightYellow)));

        Color blue = Color.LightBlue;
        SVGPunch bluePunch = new(new PunchSettings(blue));
        string bluePaper = bluePunch.Punch(data, "blue");
        Assert.IsTrue(bluePaper.Contains(ColorTranslator.ToHtml(blue)));

        Color pink = Color.LightPink;
        SVGPunch pinkPunch = new(new PunchSettings(pink));
        string pinkPaper = pinkPunch.Punch(data, "pink");
        Assert.IsTrue(pinkPaper.Contains(ColorTranslator.ToHtml(pink)));
    }
}
