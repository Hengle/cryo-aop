using CryoAOP.Core;
using CryoAOP.Core.Extensions;
using CryoAOP.TestAssembly;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    [TestFixture]
    public class MethodInspectorTestDeepScope : MethodInspectorTestBase
    {
        protected override string InterceptType
        {
            get { return "CryoAOP.TestAssembly.TestMethodInterceptorTypeDeepScope"; }
        }

        protected override string DebuggingInterceptorMethod
        {
            get { return "InterceptMethod"; }
        }

        protected override MethodInterceptionScope MethodInterceptionScope
        {
            get { return MethodInterceptionScope.Deep; }
        }

        [Test]
        public void Should_intercept_internal_calls_when_doing_deep_interception()
        {
            var wasInterceptedWithInternalCall = false;
            var methodCallToIntercept = InterceptedAssembly.GetNonGenericMethodInfo<TestMethodInterceptorTypeDeepScope>("CallToIntercept");
            GlobalInterceptor.MethodIntercepter += (invocation) => { wasInterceptedWithInternalCall = true; };
            methodCallToIntercept.AutoInstanceInvoke();
            Assert.That(wasInterceptedWithInternalCall);
        }
    }
}