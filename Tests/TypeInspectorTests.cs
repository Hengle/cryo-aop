using System;
using CryoAOP.Core;
using CryoAOP.Core.Exceptions;
using CryoAOP.TestAssembly;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    [TestFixture]
    public class TypeInspectorTests : AssemblyInspectorTests
    {
        protected TypeInspector TypeInspector;
        private Type typeThatShouldBeIntercepted;

        public override void Setup_Fixture()
        {
            base.Setup_Fixture();
            typeThatShouldBeIntercepted = typeof(TypeThatShouldBeIntercepted);
            TypeInspector = AssemblyInspector.FindType(typeThatShouldBeIntercepted);
        }

        [Test]
        public void Should_find_method_for_known_type()
        {
            var methodInfo = typeThatShouldBeIntercepted.GetMethod("HavingMethodWithNoArgsAndNoReturnType");
            var methodInspector = TypeInspector.FindMethod(methodInfo);
            Assert.That(methodInspector, Is.Not.Null);
        }

        [Test]
        [ExpectedException(typeof(MethodNotFoundException))]
        public void Should_throw_if_method_not_found()
        {
            TypeInspector.FindMethod("Some method that cannot be found");
        }
    }
}