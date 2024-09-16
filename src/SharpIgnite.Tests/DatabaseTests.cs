using NUnit.Framework;
using System;
using System.Data;
using System.Data.SqlClient;
using S = SharpIgnite;

namespace SharpIgnite.Tests
{
    [TestFixture]
    public class DatabaseTests
    {
        IDatabaseAdapter databaseAdapter;
        Database db;

        [SetUp]
        public void Setup()
        {
            databaseAdapter = new DummyDatabaseAdapter(new SqlQueryBuilder());
            db = S.Database.Instance.Adapter(databaseAdapter); // new S.Database(databaseDriver);
        }

        [Test]
        public void TestDropAndCreate()
        {
            db.Drop("Person");
            Console.WriteLine(db.LastQuery);

            db.Column("PersonID", DbType.Int32, true, true)
                .Column("Name")
                .Column("Address")
                .Column("Age", DbType.Int32)
                .Create("Person");
            Console.WriteLine(db.LastQuery);
        }

        [Test]
        public void TestInsert()
        {
            var data = S.Array.New()
                .Add("PersonID", new Random().Next(9999))
                .Add("Name", Guid.NewGuid().ToString())
                .Add("Address", "Philippines")
                .Add("Age", new Random().Next(80));

            db.Insert("Person", data);

            Console.WriteLine(db.LastQuery);
        }

        [Test]
        public void TestSelect()
        {
            db.Select("av.ValueInt")
                .From("AnswerValue av")
                .Join("Answer a", "a.AnswerID = av.AnswerID")
                .Join("Question q", "q.QuestionID = av.QuestionID")
                .Join("[Option] o", "o.OptionID = av.OptionID")
                .Where("a.AnswerID", 405300)
                .Where("a.ProjectRoundUserID", 68748)
                .GroupBy("a.ProjectRoundUserID");

            var r = db.Result<AnswerValue>();
            Console.WriteLine(r);
            Console.WriteLine(db.LastQuery);

            int c = db.Count();
            Console.WriteLine(c);
        }

        [Test]
        public void TestDelete()
        {
            int x = db.Delete("OptionComponent", "OptionComponentID >= 2800");
            Console.WriteLine(db.LastQuery);
            x = db.Delete("OptionComponentLang", "OptionComponentLangID >= 8903");
            Console.WriteLine(x);
        }

        [Test]
        public void TestUpdate()
        {
            db.Set("Name", "Juan Dela Cruzx");
            db.Set("Address", "Philippines");
            db.Where("PersonID = 1");
            int x = db.Update("Person");

            Console.WriteLine(db.LastQuery);
        }

        [Test]
        public void TestCount()
        {
            int? questionContainerID = 1;
            string variableName = "Some variable name";

            //var db = new Database(databaseDriver);
            db.QueryChanged += delegate (object sender, DatabaseEventArgs e) {
                Console.WriteLine(e.Query);
            };
            if (questionContainerID == null) {
                db.Where("QuestionContainerID IS NULL");
            } else {
                db.Where("QuestionContainerID = " + questionContainerID.Value);
                db.Where("Variablename = '" + variableName + "'");
            }
            db.From("QuestionContainer").Count();

            string query = @"SELECT COUNT(*)
FROM QuestionContainer
WHERE QuestionContainerID = 1
AND Variablename = 'Some variable name'";

            //Assert.IsTrue(query.Equals(db.LastQuery, StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void b()
        {
            Application.Instance.Database.LoadConnectionString(@"Server=.\SQLExpress;Database=eform;Integrated Security=True;");

            int i = 1;
            foreach (var q in Question.Find(S.Array.New().Add("QuestionID", 3722))) {
                Console.WriteLine(q.Internal);
                Console.WriteLine(i++);
            }
        }

        [Test]
        public void a()
        {
            db.QueryChanged += delegate (object sender, DatabaseEventArgs e) {
                //                Console.WriteLine(e.Query);
            };
            var news = db
                .Select("*")
                .From("AdminNews")
                .Limit(3)
                .Result<AdminNews>();

            foreach (var n in news) {
                Console.WriteLine(n.News);
            }
        }

        [Test]
        public void TestResult()
        {
            var users = db
                .Select("*")
                .From("[User]")
                .OrderBy("UserID")
                .OrderBy("Username", "DESC")
                .Limit(5)
                .Result<User>();
            foreach (var u in users) {
                Console.WriteLine(u.Id + "\t" + u.Name + "\t" + u.Email);
            }
        }

        [Test]
        public void TestQuery()
        {
            var users = db.Query<User>("SELECT * FROM [User]");
            foreach (var u in users) {
                Console.WriteLine(u.Id + "\t" + u.Username + "\t" + u.Email);
            }
            Console.WriteLine(db.LastQuery);
        }

        [Test]
        public void TestRow()
        {
            var user = db
                .Select("*")
                .From("[User]")
                .Where(new { UserID = 1 })
                .Row<User>();
            Console.WriteLine(db.LastQuery);
        }

        [Test]
        public void TestQueryFirst()
        {
            var user = db.QueryFirst<User>("SELECT * FROM [User]");
            Console.WriteLine(user.Username);
        }

        [Test]
        public void TestQueryFromFile()
        {
            var connectionString = @"database=eform2;server=.\SQLEXPRESS;Trusted_Connection=True;";
            var databaseDriver = new SqlDatabaseAdapter(connectionString);
            //db = new Database(databaseDriver);
            db.QueryFromFile(@"C:\Users\Ian\Downloads\Option.sql");
        }
    }

    public class DummyDatabaseAdapter : DatabaseAdapter
    {
        public DummyDatabaseAdapter(ISqlQueryBuilder queryBuilder)
        {
            this.QueryBuilder = queryBuilder;
        }

        public override IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString); // Just a dummy
        }

        public override int NonQuery(string query, params IDataParameter[] parameters)
        {
            return 0;
        }

        public override IDataReader Reader(string query, params IDataParameter[] parameters)
        {
            return null;
        }

        public override T Scalar<T>(string query, params IDataParameter[] parameters)
        {
            return default(T);
        }
    }

    public class User : Model<User>
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class AdminNews
    {
        public string News { get; set; }
    }

    public class Question : Model<Question>
    {
        public string Internal { get; set; }
    }

    public class AnswerValue
    {

    }
}