using CryoAOP.Core;
using CryoAOP.TestAssembly;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    [TestFixture]
    public class PropertyTests
    {
        #region Setup/Teardown

        [SetUp]
        public void Should_clear_intercepts_between_calls()
        {
            Intercept.Clear();
        }

        #endregion

        [Test]
        public void Should_intercept_property_which_is_a_value_type()
        {
            var interceptorWasCalled = false;
            Intercept.Call +=
                (invocation) => { interceptorWasCalled = true; };

            var instance = new PropertyInterceptorTarget();
            instance.SomeInteger = 1;

            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_intercept_property_which_is_a_value_type_defined_by_attribute()
        {
            var interceptorWasCalled = false;
            Intercept.Call +=
                (invocation) => { interceptorWasCalled = true; };

            var instance = new PropertyInterceptorTarget();
            instance.SomeIntegerWithAttribute = 1;

            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_intercept_static_property_which_is_a_value_type_defined_by_attribute()
        {
            var interceptorWasCalled = false;
            Intercept.Call +=
                (invocation) => { interceptorWasCalled = true; };

            PropertyInterceptorTarget.SomeStaticIntegerWithAttribute = 1;

            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_intercept_static_property_which_is_a_value_type_using_getter_defined_by_attribute()
        {
            var interceptorWasCalled = false;
            Intercept.Call +=
                (invocation) => { interceptorWasCalled = true; };

            var i = PropertyInterceptorTarget.SomeStaticIntegerWithAttribute;

            Assert.That(interceptorWasCalled);
        }
    }
}