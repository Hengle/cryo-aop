using System.Linq;
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
        private const string interceptedAssembly = "CryoAOP.TestAssembly_Intercepted.dll";

        [Test]
        public void Should_intercept_method_and_call_using_reflection()
        {
            var methodInspector = 
                new[]
                    {
                        "CryoAOP.TestAssembly",
                        "TypeThatShouldBeIntercepted",
                        "HavingMethodWithArgsAndNoReturnType"
                    }.GetMethod();

            methodInspector.InterceptMethod("Test_Intercepted_");
            methodInspector.Write(interceptedAssembly);

            var assembly = Assembly.LoadFrom(interceptedAssembly);
            var interceptedType = assembly.FindType(typeof (TypeThatShouldBeIntercepted).FullName);

            var methodInfo = interceptedType.GetMethod("HavingMethodWithArgsAndNoReturnType");
            methodInfo.Invoke(1, "2", 3); 
        }
    }
}