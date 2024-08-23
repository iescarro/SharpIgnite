using System;
using NUnit.Framework;

namespace SharpIgnite.Tests
{
    [TestFixture]
    public class LoggingHelperTests
    {
        [Test]
        public void TestMethod()
        {
            Log.Info("This is info");
            Log.Debug("This is debug");
            Log.Error("This is error");
        }
    }
}
