using System;
using System.Reflection;
using CryoAOP.Core;
using CryoAOP.Core.Extensions;
using CryoAOP.TestAssembly;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    public static class MethodInspectorTestsExtensions
    {
        public static MethodInfo GetNonGenericMethodInfo(this Assembly assembly, string nonGenericMethodName)
        {
            var interceptedType = assembly.FindType(typeof (TypeThatShouldBeIntercepted).FullName);
            return interceptedType.GetMethod(nonGenericMethodName);
        }

        public static MethodInfo GetGenericMethodInfo(this Assembly assembly, string nonGenericMethodName, params Type[] genericTypeParameter)
        {
            var interceptedType = assembly.FindType(typeof (TypeThatShouldBeIntercepted).FullName);
            var genericMethodInfo = interceptedType.GetMethod(nonGenericMethodName);
            return genericMethodInfo.MakeGenericMethod(genericTypeParameter);
        }

        public static void AssertResultsFor(this Assembly assembly, NonGenericInfo nonGenericInfo)
        {
            MethodInfo methodInfo;
            var interceptCount = 0;
            MethodInvocation methodInvocation = null;
            if (nonGenericInfo is GenericInfo)
            {
                var genericInfo = ((GenericInfo) nonGenericInfo);
                methodInfo = assembly.GetGenericMethodInfo(genericInfo.MethodName, genericInfo.GenericTypes);
            }
            else 
                methodInfo = assembly.GetNonGenericMethodInfo(nonGenericInfo.MethodName);

            GlobalInterceptor.MethodIntercepter +=
                (i) =>
                {
                    if (nonGenericInfo.Invocation != null)
                        nonGenericInfo.Invocation(i);

                    methodInvocation = i;
                    interceptCount++;
                };

            var result = methodInfo.AutoInstanceInvoke(nonGenericInfo.MethodArgs);

            Assert.That(
                interceptCount,
                Is.EqualTo(methodInvocation.InvocationCancelled ? 1 : 2),
                "Either pre- or post- invocation failed, interception count should be 2 unless the invocation was cancelled ... ");
            
            if (nonGenericInfo.Assertion != null)
                nonGenericInfo.Assertion(result);
        }
    }
}