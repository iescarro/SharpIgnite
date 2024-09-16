using NUnit.Framework;
using System;

namespace SharpIgnite.Tests
{
    [TestFixture]
    public class ModelTests
    {
        int id = 6;

        [SetUp]
        public void Setup()
        {
            Application.Instance.Database.QueryChanged += delegate (object sender, DatabaseEventArgs e) {
                Console.WriteLine(e.Query);
            };
        }

        [Test]
        public void TestValid()
        {
            var p = new Person();
            Assert.IsFalse(p.IsValid);

            p.FirstName = "Juan";
            Assert.IsTrue(p.IsValid);

            p.FirstName = "Evil";
            Assert.IsFalse(p.IsValid);
        }

        [Test]
        public void TestSave()
        {
            var n = new News();
            n.Date = DateTime.Now;
            n.Content = "A very big news today!";
            n.Save();

            Console.WriteLine(n);
        }

        [Test]
        public void TestUpdate()
        {
            var n = News.Read(new { Id = 1 });
            n.Content = "Updated news content";
            n.Update();
        }

        [Test]
        public void TestDelete()
        {
            var n = News.Read(new { Id = 1 });
            n.Delete();
        }

        [Test]
        public void TestAll()
        {
            var news = News.All();
            foreach (var n in news) {
                Console.WriteLine(n);
            }
        }

        [Test]
        public void TestFind()
        {
            var news = News.Find(new { Id = 1 });
            foreach (var n in news) {
                Console.WriteLine(n);
            }

            news = News.Where(new { Category = "Science" })
                .OrderBy("Date", "DESC")
                .Result<News>();

            news = News.Limit(3)
                .OrderBy("Date", "DESC")
                .Result<News>();
        }
    }

    [Table("AdminNews")]
    public class News : Model<News>
    {
        [Column("NewsID")]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public string Content { get; set; }
    }

    public class Person : Model<Person>
    {
        [Required]
        public string FirstName { get; set; }

        public Person()
        {
            ValidatesWith(new GoodnessValidator());
        }
    }

    public class GoodnessValidator : Validator
    {
        public override void Validate(object obj)
        {
            var p = obj as Person;
            if (p.FirstName == "Evil") {
                p.Errors.Add("This person is evil");
            }
        }
    }

    public class Post : Model<Post>
    {
        public int PostID { get; set; }

        [Required]
        public string Title { get; set; }
    }

    public class Comment : Model<Comment>
    {
        [Column("Comment")]
        public string CommentText { get; set; }
    }
}
