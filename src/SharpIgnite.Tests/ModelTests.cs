using NUnit.Framework;
using System;
using S = SharpIgnite;

namespace SharpIgnite.Tests
{
    [TestFixture]
    public class ModelTests
    {
        int id = 6;
        Database db;

        [SetUp]
        public void Setup()
        {
            db = WebApplication.Instance.Database;
            db.QueryChanged += delegate (object sender, DatabaseEventArgs e) {
                Console.WriteLine(e.Query);
            };
        }

        static readonly object[] DatabaseDrivers = {
            new object[] { new SqlDatabaseDriver(), "SqlConnection" },
            new object[] { new SQLiteDatabaseDriver(), "DbConnection" },
            new object[] { new MySqlDatabaseDriver(), "MySqlConnection" },
        };

        [Test, TestCaseSource(nameof(DatabaseDrivers))]
        public void TestSave(IDatabaseDriver databaseDriver, string connectionName)
        {
            db.Adapter(databaseDriver).Load(connectionName);
            var p = new Post {
                Title = "This is a post"
            };
            p.Save();
            Console.WriteLine(p);
        }

        [Test, TestCaseSource(nameof(DatabaseDrivers))]
        public void TestUpdate(IDatabaseDriver databaseDriver, string connectionName)
        {
            db.Adapter(databaseDriver).Load(connectionName);
            var p = Post.Read(S.Array.New().Add("Id", 1));
            p.Title = "This is an updated post";
            p.Update();
        }

        [Test, TestCaseSource(nameof(DatabaseDrivers))]
        public void TestDelete(IDatabaseDriver databaseDriver, string connectionName)
        {
            db.Adapter(databaseDriver).Load(connectionName);
            var p = Post.Read(S.Array.New().Add("Id", id));
            p.Delete();
        }

        [Test, TestCaseSource(nameof(DatabaseDrivers))]
        public void TestAll(IDatabaseDriver databaseDriver, string connectionName)
        {
            db.Adapter(databaseDriver).Load(connectionName);
            var posts = Post.All();
            foreach (var p in posts) {
                Console.WriteLine(p);
            }
        }

        [Test, TestCaseSource(nameof(DatabaseDrivers))]
        public void TestOrderBy(IDatabaseDriver databaseDriver, string connectionName)
        {
            db.Adapter(databaseDriver).Load(connectionName);
            var posts = Post.OrderBy("Id", "DESC").Limit(3).Result<Post>();
            foreach (var p in posts) {
                Console.WriteLine(p);
            }
        }

        [Test, TestCaseSource(nameof(DatabaseDrivers))]
        public void TestFind(IDatabaseDriver databaseDriver, string connectionName)
        {
            db.Adapter(databaseDriver).Load(connectionName);
            var w = new S.Array("Id", id);
            var posts = Post.Find(w);
            foreach (var p in posts) {
                Console.WriteLine(p);
            }
        }
    }

    [Table()]
    public class Post : Model<Post>
    {
        [Column("Id", true, true)] public int PostID { get; set; }
        public string Title { get; set; }
    }

    [Table("Comment")]
    public class Comment : Model<Comment>
    {
        public Post Post { get; set; }
        [Column("CommentText")] public string Text { get; set; }
    }
}
