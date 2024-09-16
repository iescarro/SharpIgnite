using System;
using HW.Core.Util.SharpIgnite.Cli;
using NUnit.Framework;

namespace HW.Core.Tests.Util.SharpIgnite.Cli
{
    [TestFixture]
    public class MakeTests
    {
        [Test]
        public void TestMigrationFromDB()
        {
            var m = new Make();
            m.MigrationFromDB(@"database=healthWatch2;server=.\SQLEXPRESS;Trusted_Connection=True;");
        }
    }
}
