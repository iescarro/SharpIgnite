using NUnit.Framework;
using S = SharpIgnite;

namespace SharpIgnite.Tests
{
    [TestFixture]
    public class ArrayTests
    {
        [Test]
        public void TestAddRange()
        {
            var a = new S.Array();
            a.AddRange(S.Array.New().Add("Id", 1).Add("Name", "John Doe"));

            Assert.AreEqual(2, a.Count);
            Assert.IsNull(a["SomethingNull"]);
            Assert.AreEqual(1, a["Id"]);
        }

        [Test]
        public void TestToString()
        {
            var a = S.Array.New().Add("Id", 1).Add("Name", "John");
            Assert.AreEqual("Id = 1, Name = 'John'", a.ToString());
        }

        [Test]
        public void TestAddIf()
        {
            var a = S.Array.New().Add("Id", 1);
            a.AddIf(true, "Name", "John");
            a.AddIf(false, "Address", "Cebu City");

            Assert.AreEqual(2, a.Count);
        }
    }
}
