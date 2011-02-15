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
        private static Assembly assembly;

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
            assembly = Assembly.LoadFrom(interceptedOutputAssembly);
        }

        private static MethodInfo GetNonGenericMethodInfo(string nonGenericMethodName)
        {
            var interceptedType = assembly.FindType(typeof (TypeThatShouldBeIntercepted).FullName);
            return interceptedType.GetMethod(nonGenericMethodName);
        }

        [Test]
        public void Should_intercept_method_and_call()
        {
            var interceptCount = 0;
            var methodInfo = GetNonGenericMethodInfo("HavingMethodWithArgsAndNoReturnType");

            GlobalInterceptor.MethodIntercepter += (invocation) => { interceptCount++; };

            var result = methodInfo.Invoke(1, "2", 3);
            Assert.That(interceptCount, Is.EqualTo(2));
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Should_intercept_method_and_cancel_invocation_returning_fake_result()
        {
            var methodInfo = GetNonGenericMethodInfo("HavingMethodWithArgsAndStringReturnType");

            GlobalInterceptor.MethodIntercepter += (invocation) =>
            {
                if (invocation.InvocationType == MethodInvocationType.BeforeInvocation)
                {
                    invocation.CancelInvocation();
                    invocation.Result = "Fake Result";
                }
            };

            var result = methodInfo.Invoke(1, "2", 3);
            Assert.That(result, Is.EqualTo("Fake Result"));
        }

        [Test]
        public void Should_intercept_method_with_class_args_and_call()
        {
            var interceptCount = 0;
            var methodInfo = GetNonGenericMethodInfo("HavingMethodWithClassArgsAndClassReturnType");
            var args = assembly.CreateInstance("CryoAOP.TestAssembly.MethodParameterClass");

            GlobalInterceptor.MethodIntercepter += (invocation) => { interceptCount++; };

            var result = methodInfo.Invoke(args);
            Assert.That(interceptCount, Is.EqualTo(2));
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Should_intercept_method_and_call_using_reflection_changing_return_value()
        {
            var methodInfo = GetNonGenericMethodInfo("HavingMethodWithArgsAndStringReturnType");

            GlobalInterceptor.MethodIntercepter += (invocation) =>
                                                       {
                                                           if (invocation.InvocationType == MethodInvocationType.AfterInvocation)
                                                               invocation.Result = "Intercepted Result";
                                                       };

            var result = methodInfo.Invoke(1, "2", 3);
            Assert.That(result, Is.EqualTo("Intercepted Result"));
        }

        [Test]
        public void Should_intercept_method_and_call_with_result()
        {
            var interceptCount = 0;
            var methodInfo = GetNonGenericMethodInfo("HavingMethodWithArgsAndStringReturnType");

            GlobalInterceptor.MethodIntercepter += (invocation) => { interceptCount++; };

            var result = methodInfo.Invoke(1, "2", 3);
            Assert.That(interceptCount, Is.EqualTo(2));
            Assert.That(result, Is.EqualTo("1, 2, 3"));
        }
    }
}