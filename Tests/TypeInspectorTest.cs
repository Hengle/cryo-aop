using System;
using CryoAOP.Core;
using CryoAOP.Core.Exceptions;
using CryoAOP.TestAssembly;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    [TestFixture]
    public class TypeInspectorTest
    {
        protected string TestAssembly;
        protected AssemblyInspector AssemblyInspector;
        protected TypeInspector TypeInspector;
        private Type typeThatShouldBeIntercepted;

        [TestFixtureSetUp]
        public virtual void Setup_Fixture()
        {
            TestAssembly = "CryoAOP.TestAssembly.dll";
            AssemblyInspector = new AssemblyInspector(TestAssembly);
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