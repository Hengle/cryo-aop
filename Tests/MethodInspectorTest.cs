using CryoAOP.Core;
using CryoAOP.Core.Extensions;
using CryoAOP.TestAssembly;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    [TestFixture]
    public class MethodInspectorTest : MethodInspectorTestBase
    {
        // ! WARNING ! There is an explicit failing test in the base fixture, it was to test method switching in the test classes see ("Should_intercept_method_and_cancel_invocation_returning_fake_result")
        protected override string InterceptMethod
        {
             get { return null; }
            //get { return "GenericMethodWithTwoGenericParameters"; }
        }

        [Test] // See, PanicStationFixture ... this is possible ... 
        public void Should_call_generic_method_with_two_generic_parameters()
        {
            //var i = 1;
            //var j = new MethodParameterClass();
            //var interceptorCount = 0;
            //var methodInfo = InterceptedAssembly.GetGenericMethodInfo("GenericMethodWithTwoGenericParameters", typeof (int), typeof (MethodParameterClass));

            //GlobalInterceptor.MethodIntercepter += (invocation) =>
            //                                           {
            //                                               Assert.That(i == (int) invocation.ParameterValues[0]);
            //                                               Assert.That(j == invocation.ParameterValues[1]);

            //                                               interceptorCount++;
            //                                           };

            //methodInfo.AutoInstanceInvoke(i, j);
        }
    }
}