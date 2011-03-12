using CryoAOP.Core;
using CryoAOP.TestAssembly;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    [TestFixture]
    public class PropertyInterceptorTests
    {
        [SetUp]
        public void Should_clear_intercepts_between_calls()
        {
            Intercept.Clear();
        }
    }
}