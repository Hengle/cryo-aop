using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CryoAOP.Core;
using CryoAOP.Core.Extensions;
using CryoAOP.TestAssembly;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.PE;
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
            GlobalInterceptor.Clear();
        }

        #endregion

        private const string interceptedOutputAssembly = "CryoAOP.TestAssembly_Intercepted.dll";

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            //Interception.RegisterType("CryoAOP.TestAssembly", "CryoAOP.TestAssembly.TypeThatShouldBeIntercepted", "GenericMethodWithGenericParamsAndValueReturnType", interceptedOutputAssembly);
            Interception.RegisterType("CryoAOP.TestAssembly", "CryoAOP.TestAssembly.TypeThatShouldBeIntercepted", interceptedOutputAssembly);
            assembly = Assembly.LoadFrom(interceptedOutputAssembly);
        }

        private static MethodInfo GetNonGenericMethodInfo(string nonGenericMethodName)
        {
            var interceptedType = assembly.FindType(typeof(TypeThatShouldBeIntercepted).FullName);
            return interceptedType.GetMethod(nonGenericMethodName);
        }

        private static MethodInfo GetGenericMethodInfo(string nonGenericMethodName, Type genericTypeParameter)
        {
            var interceptedType = assembly.FindType(typeof(TypeThatShouldBeIntercepted).FullName);
            MethodInfo genericMethodInfo = interceptedType.GetMethod(nonGenericMethodName);
            return genericMethodInfo.MakeGenericMethod(genericTypeParameter);
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

        [Test]
        public void Should_call_to_generic_method()
        {
            var interceptorCount = 0;
            var methodInfo = GetGenericMethodInfo("GenericMethod", typeof(MethodParameterClass));

            GlobalInterceptor.MethodIntercepter += (invocation) => { interceptorCount++; };

            methodInfo.Invoke();
            Assert.That(interceptorCount, Is.EqualTo(2));
        }

        [Test]
        public void Should_call_to_generic_method_with_generic_parameters()
        {
            var interceptorCount = 0;
            var methodInfo = GetGenericMethodInfo("GenericMethodWithGenericParameters", typeof (MethodParameterClass));

            GlobalInterceptor.MethodIntercepter += (invocation) => { interceptorCount++; };

            methodInfo.Invoke(new MethodParameterClass());
            Assert.That(interceptorCount, Is.EqualTo(2));
        }

        [Test]
        public void Should_call_to_generic_method_with_generic_parameters_and_generic_return_type()
        {
            var interceptorCount = 0;
            var methodInfo = GetGenericMethodInfo("GenericMethodWithGenericParametersAndGenericReturnType", typeof(MethodParameterClass));

            GlobalInterceptor.MethodIntercepter += (invocation) => { interceptorCount++; };

            var parameterClass = new MethodParameterClass();
            var result = methodInfo.Invoke(parameterClass);
            Assert.That(interceptorCount, Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(parameterClass));
        }

        [Test]
        public void Should_call_to_generic_with_generic_parameters_and_value_types()
        {
            int i = 0;
            double j = 0;
            var interceptorCount = 0;
            var methodInfo = GetGenericMethodInfo("GenericMethodWithGenericParametersAndValueTypeArgs", typeof(MethodParameterClass));

            GlobalInterceptor.MethodIntercepter += (invocation) =>
                                                       {
                                                           i = (int)invocation.ParameterValues[1];
                                                           j = (double)invocation.ParameterValues[2];
                                                           interceptorCount++;
                                                       };

            var parameterClass = new MethodParameterClass();
            methodInfo.Invoke(parameterClass, 1, 2);
            Assert.That(i, Is.EqualTo(1));
            Assert.That(j, Is.EqualTo(2));
            Assert.That(interceptorCount, Is.EqualTo(2));
        }

        [Test]//, Ignore("The return type being assigned to the method invocation needs to be revised ... wrong!")]
        public void Should_call_generic_with_all_kinds_of_parameters_and_return_a_value_type()
        {
            int i = 0;
            double j = 0;
            var interceptorCount = 0;
            var methodInfo = GetGenericMethodInfo("GenericMethodWithGenericParamsAndValueReturnType", typeof(MethodParameterClass));

            GlobalInterceptor.MethodIntercepter += (invocation) =>
            {
                i = (int)invocation.ParameterValues[1];
                j = (double)invocation.ParameterValues[2];
                interceptorCount++;
            };

            var parameterClass = new MethodParameterClass();
            var result = methodInfo.Invoke(parameterClass, 1, 2);
            Assert.That(i, Is.EqualTo(1));
            Assert.That(j, Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(j));
            Assert.That(interceptorCount, Is.EqualTo(2));
        }
    }
}