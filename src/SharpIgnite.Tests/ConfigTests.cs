using NUnit.Framework;

namespace SharpIgnite.Tests
{
    [TestFixture]
    public class ConfigTests
    {
        [Test]
        public void TestGet()
        {
            var driver = Config.Get("DB_CONNECTION");
            Assert.AreEqual("sqlite", driver);

            var x = Config.Get("SettingNotFound");
            Assert.IsNull(x);

            int costParameter = Config.Get<int>("CostParameter");
            Assert.AreEqual(13, costParameter);
        }
    }
}
