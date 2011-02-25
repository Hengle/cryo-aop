using CryoAOP.Core.Extensions;
using NUnit.Framework;

namespace CryoAOP.Tests.Extensions
{
    [TestFixture]
    public class StringExtensionsTest
    {
        [Test]
        public void Should_format_string_correctly()
        {
            var value = "This is a {0}".FormatWith("string");
            Assert.That(value, Is.EqualTo("This is a string"));
        }

        [Test]
        [ExpectedException]
        public void Should_throw_if_no_arguments()
        {
            var value = "This is a {0}".FormatWith();
        }
    }
}