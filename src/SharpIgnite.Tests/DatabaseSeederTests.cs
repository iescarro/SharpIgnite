using NUnit.Framework;

namespace SharpIgnite.Tests
{
    [TestFixture]
    public class DatabaseSeederTests
    {
        IDatabaseAdapter databaseAdapter;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestPostSeeder()
        {
            var s = new PostSeeder();
            s.Rollback();
            s.Run();
        }
    }

    public class PostSeeder : Seeder
    {
        public override void Run()
        {
            DB.Table("Post")
                .Insert(new[] {
                    new { Title = "A post" },
                    new { Title = "Another post" },
                    new { Title = "Yet another post" },
                    new { Title = "Of course, this one is also a post" },
                    new { Title = "Right? Another post" },
                });
        }

        public override void Rollback()
        {
            DB.Table("Post").Truncate();
        }
    }
}