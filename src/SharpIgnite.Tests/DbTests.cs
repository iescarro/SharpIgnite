using NUnit.Framework;
using System;

namespace SharpIgnite.Tests
{
    [TestFixture]
    public class DbTests
    {
        [Test]
        public void Test()
        {
            DB.NonQuery("DELETE FROM Post");
            DB.NonQuery("INSERT INTO Post(Title) VALUES('Initial post of the day. Wow!')");
            using (var rs = DB.Reader("SELECT Id, Title FROM Post")) {
                if (rs.Read()) {
                    int id = DB.GetInt32(rs, 0);
                    string title = DB.GetString(rs, 1);
                    Console.WriteLine($"Id: {id}, Title: {title}");
                }
            }
            int postID = DB.Scalar<int>("SELECT Id FROM Post");
            DB.NonQuery($"UPDATE Post SET Title = 'Changed title from the previous post' WHERE Id = {postID}");
        }
    }
}
