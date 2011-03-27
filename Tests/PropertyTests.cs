//CryoAOP. Aspect Oriented Framework for .NET.
//Copyright (C) 2011  Gavin van der Merwe (fir3pho3nixx@gmail.com)

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

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

        [Test]
        public void Should_intercept_static_property_which_is_a_value_type_defined_by_attribute()
        {
            var interceptorWasCalled = false;
            Intercept.Call +=
                (invocation) =>
                {
                    interceptorWasCalled = true;
                };

            PropertyInterceptorTarget.SomeStaticIntegerWithAttribute = 1;

            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_intercept_static_property_which_is_a_value_type_using_getter_defined_by_attribute()
        {
            var interceptorWasCalled = false;
            Intercept.Call +=
                (invocation) =>
                    {
                        interceptorWasCalled = true;
                    };
            
            var i = PropertyInterceptorTarget.SomeStaticIntegerWithAttribute;
            
            Assert.That(interceptorWasCalled);
        }
    }
}