using NUnit.Framework;
using SharpIgnite;

namespace HW.Core.Tests.Util.SharpIgnite
{
    [TestFixture]
    public class ValidatorTests
    {
        [Test]
        public void TestEmail()
        {
            var v = new EmailValidator("ian.escarro@gmail.com");
            Assert.IsTrue(v.IsValid);

            v = new EmailValidator("some.com");
            Assert.IsFalse(v.IsValid);
        }

        [Test]
        public void TestRequired()
        {
            var v = new RequiredValidator("");
            Assert.IsFalse(v.IsValid);

            v = new RequiredValidator(null);
            Assert.IsFalse(v.IsValid);

            v = new RequiredValidator("There is value");
            Assert.IsTrue(v.IsValid);
        }
    }
}
