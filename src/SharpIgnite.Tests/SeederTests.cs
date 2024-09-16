using NUnit.Framework;

namespace SharpIgnite.Tests
{
    [TestFixture]
    public class SeederTests
    {
        [Test]
        public void TestRun()
        {
            var s = new UsersSeeder();
            s.Run();
        }
    }

    public class UsersSeeder : Seeder
    {
        public override void Run()
        {
        }
    }
}