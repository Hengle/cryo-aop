using System;
using CryoAOP.Core;
using CryoAOP.Core.Exceptions;
using CryoAOP.TestAssembly;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    [TestFixture]
    public class TypeInterceptTest
    {
        protected string TestAssembly;
        protected AssemblyIntercept AssemblyIntercept;
        protected TypeIntercept TypeIntercept;
        private Type typeThatShouldBeIntercepted;

        [TestFixtureSetUp]
        public virtual void Setup_Fixture()
        {
            TestAssembly = "CryoAOP.TestAssembly.dll";
            AssemblyIntercept = new AssemblyIntercept(TestAssembly);
            typeThatShouldBeIntercepted = typeof(TestMethodInterceptorType);
            TypeIntercept = AssemblyIntercept.FindType(typeThatShouldBeIntercepted);
        }

        [Test]
        public void Should_find_method_for_known_type()
        {
            var methodInfo = typeThatShouldBeIntercepted.GetMethod("HavingMethodWithNoArgsAndNoReturnType");
            var methodInspector = TypeIntercept.FindMethod(methodInfo);
            Assert.That(methodInspector, Is.Not.Null);
        }

        [Test]
        [ExpectedException(typeof(MethodNotFoundException))]
        public void Should_throw_if_method_not_found()
        {
            TypeIntercept.FindMethod("Some method that cannot be found");
        }
    }
}