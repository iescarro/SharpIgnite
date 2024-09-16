using NUnit.Framework;
using System.Data;

namespace SharpIgnite.Tests
{
    [TestFixture]
    public class MigrationTests
    {
        Migration m;

        [SetUp]
        public void Setup()
        {
            m = new _CreateDepartment();
        }

        [Test]
        public void TestUp()
        {
            m.Up();
        }

        [Test]
        public void TestDown()
        {
            m.Down();
        }
    }

    public class _CreateDepartment : Migration
    {
        public _CreateDepartment() : base("E96DF9E3-753F-4AF3-BF72-550209EBC8E2")
        {
        }

        public override void Up()
        {
            CreateTable("Department",
                Column("DepartmentID", DbType.Int32, true, true),
                Column("Department"),
                Column("ParentDepartmentID", DbType.Int32),
                Column("SortOrder", DbType.Int32),
                Column("DepartmentShort")
            );
        }

        public override void Down()
        {
            DropTable("Department");
        }
    }
}
