using NUnit.Framework;
using System;
using S = SharpIgnite;

namespace SharpIgnite.Tests
{
    [TestFixture]
    public class RouteTests
    {
        [Test]
        public void TestWithUri()
        {
            var uri = new Uri("http://localhost:3132/index.aspx/home");
            var r = new S.Route(uri);
            Assert.AreEqual("HomeController", r.Controller);
            Assert.AreEqual("Index", r.Action);
            Assert.AreEqual("home", r.Name);

            uri = new Uri("http://localhost:3132/index.aspx");
            r = new S.Route(uri);
            Assert.AreEqual("/", r.Name);

            uri = new Uri("http://localhost:3132/index.aspx/login");
            r = new S.Route(uri);
            Assert.AreEqual("login", r.Name);
        }

        [Test]
        public void TestWithAbsolutePath()
        {
            var r = new S.Route("/");
            Assert.AreEqual("/", r.Name);

            r = new S.Route("home/index");
            Assert.AreEqual("HomeController", r.Controller);
            Assert.AreEqual("Index", r.Action);
            Assert.AreEqual("home/index", r.Name);
        }
    }
}
