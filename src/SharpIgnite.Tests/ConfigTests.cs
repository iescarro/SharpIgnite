using NUnit.Framework;

namespace SharpIgnite.Tests
{
    [TestFixture]
    public class ConfigTests
    {
        [Test]
        public void TestItem()
        {
            var driver = Config.Item("DatabaseDriver");

            var x = Config.Item("SettingNotFound");
            Assert.IsEmpty(x);

            int costParameter = Config.Item<int>("CostParameter");
            Assert.AreEqual(13, costParameter);
        }
    }
}
