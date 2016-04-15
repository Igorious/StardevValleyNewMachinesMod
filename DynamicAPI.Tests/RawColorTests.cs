using Igorious.StardewValley.DynamicAPI.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Igorious.StardewValley.DynamicAPI.Tests
{
    [TestClass]
    public class RawColorTests
    {
        [TestMethod]
        public void CheckHexConversion()
        {
            var raw = RawColor.FromHex("12AB34");
            Assert.AreEqual(18, raw.R);
            Assert.AreEqual(171, raw.G);
            Assert.AreEqual(52, raw.B);

            var hex = raw.ToHex();
            Assert.AreEqual("12AB34", hex);
        }
    }
}
