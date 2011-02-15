using System.Reflection;
using CryoAOP.Core;
using CryoAOP.Core.Extensions;
using CryoAOP.TestAssembly;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    [TestFixture]
    public class MethodInspectorTests
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            GlobalInterceptor.ClearAll();
        }

        #endregion

        private const string interceptedOutputAssembly = "CryoAOP.TestAssembly_Intercepted.dll";

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            Interception.RegisterType("CryoAOP.TestAssembly", "CryoAOP.TestAssembly.TypeThatShouldBeIntercepted", interceptedOutputAssembly);
        }

        [Test]
        public void Should_intercept_method_and_call()
        {
            var assembly = Assembly.LoadFrom(interceptedOutputAssembly);
            var interceptedType = assembly.FindType(typeof(TypeThatShouldBeIntercepted).FullName);
            var methodInfo = interceptedType.GetMethod("HavingMethodWithArgsAndNoReturnType");

            int interceptCount = 0;

            GlobalInterceptor.MethodIntercepter += (invocation) =>
            {
                interceptCount++;
            };

            var result = methodInfo.Invoke(1, "2", 3);
            Assert.That(interceptCount, Is.EqualTo(2));
            Assert.That(result, Is.Null);
            
        }

        [Test]
        public void Should_intercept_method_and_call_with_result()
        {
            var assembly = Assembly.LoadFrom(interceptedOutputAssembly);
            var interceptedType = assembly.FindType(typeof(TypeThatShouldBeIntercepted).FullName);
            var methodInfo = interceptedType.GetMethod("HavingMethodWithArgsAndStringReturnType");

            int interceptCount = 0;

            GlobalInterceptor.MethodIntercepter += (invocation) =>
            {
                interceptCount++;
            };

            var result = methodInfo.Invoke(1, "2", 3);
            Assert.That(interceptCount, Is.EqualTo(2));
            Assert.That(result, Is.EqualTo("1, 2, 3"));
        }

        [Test]
        public void Should_intercept_method_and_call_using_reflection_changing_return_value()
        {
            var assembly = Assembly.LoadFrom(interceptedOutputAssembly);
            var interceptedType = assembly.FindType(typeof (TypeThatShouldBeIntercepted).FullName);
            var methodInfo = interceptedType.GetMethod("HavingMethodWithArgsAndStringReturnType");

            GlobalInterceptor.MethodIntercepter += (invocation) =>
                                                       {
                                                           if (invocation.InvocationType == MethodInvocationType.AfterInvocation)
                                                               invocation.Result = "Intercepted Result";
                                                       };

            var result = methodInfo.Invoke(1, "2", 3);
            Assert.That(result, Is.EqualTo("Intercepted Result"));
        }
    }
}