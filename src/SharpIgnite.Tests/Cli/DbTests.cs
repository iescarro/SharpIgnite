using System;
using HW.Core.Util.SharpIgnite.Cli;
using NUnit.Framework;

namespace HW.Core.Tests.Util.SharpIgnite.Cli
{
    [TestFixture]
    public class DbTests
    {
        [Test]
        public void TestSeed()
        {
            var db = new Db();
            db.Seed("HW.Core.dll", @"database=eform2;server=.\SQLEXPRESS;Trusted_Connection=True;");
        }
    }
}
