using System;
using System.Reflection;
using CryoAOP.Core;
using CryoAOP.Core.Extensions;
using CryoAOP.TestAssembly;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    public static class MethodInspectorTestExtensions
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

        public static void AssertResultsFor(this Assembly assembly, MethodInspectorTestMethodInfo info)
        {
            MethodInfo methodInfo;
            var interceptCount = 0;
            MethodInvocation methodInvocation = null;
            if (info is MethodInspectorTestMethodGenericInfo)
            {
                var genericInfo = ((MethodInspectorTestMethodGenericInfo) info);
                methodInfo = assembly.GetGenericMethodInfo(genericInfo.MethodName, genericInfo.GenericTypes);
            }
            else 
                methodInfo = assembly.GetNonGenericMethodInfo(info.MethodName);

            GlobalInterceptor.MethodIntercepter +=
                (i) =>
                {
                    if (info.Invocation != null)
                        info.Invocation(i);

                    methodInvocation = i;
                    interceptCount++;
                };

            var result = methodInfo.AutoInstanceInvoke(info.MethodArgs);

            Assert.That(
                interceptCount,
                Is.EqualTo(methodInvocation.InvocationCancelled ? 1 : 2),
                "Either pre- or post- invocation failed, interception count should be 2 unless the invocation was cancelled ... ");
            
            if (info.Assertion != null)
                info.Assertion(result);
        }
    }
}