using CryoAOP.Core;
using CryoAOP.TestAssembly;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    [TestFixture]
    public class PropertyTests
    {
        [SetUp]
        public void Should_clear_intercepts_between_calls()
        {
            Intercept.Clear();
        }

        [Test]
        public void Should_intercept_property_which_is_a_value_type()
        {
            var interceptorWasCalled = false;
            Intercept.Call +=
                (invocation) =>
                    {
                        interceptorWasCalled = true;
                    };

            var instance = new PropertyInterceptorTarget();
            instance.SomeInteger = 1;

            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_intercept_property_which_is_a_value_type_defined_by_attribute()
        {
            var interceptorWasCalled = false;
            Intercept.Call +=
                (invocation) =>
                {
                    interceptorWasCalled = true;
                };

            var instance = new PropertyInterceptorTarget();
            instance.SomeIntegerWithAttribute = 1;

            Assert.That(interceptorWasCalled);
        }
    }
}