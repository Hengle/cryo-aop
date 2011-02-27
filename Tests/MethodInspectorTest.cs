using System;
using CryoAOP.Core;
using CryoAOP.Core.Extensions;
using CryoAOP.TestAssembly;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    [TestFixture]
    public class MethodInspectorTest : MethodInspectorTestBase
    {
        protected override string DebuggingInterceptorMethod
        {
            get { return null; }
            //get { return "StaticMethodWithGenericAndValueTypeArgsAndNoReturnType"; }
        }

        [Test]
        public void Should_invoke_static_method_with_value_and_generic_with_no_return_type()
        {
            if (DebuggingInterceptorMethod != null) return;

            var @genericArg = new MethodParameterClass();

            var info =
                new MethodInspectorTestMethodGenericInfo(
                    methodName: "StaticMethodWithGenericAndValueTypeArgsAndNoReturnType",
                    methodArgs: new object[] { 1, genericArg },
                    genericTypes: new []{ typeof(MethodParameterClass) },
                    invocation: (invocation) =>
                    {
                        Assert.That((int)invocation.ParameterValues[0] == 1);
                        Assert.That((MethodParameterClass)invocation.ParameterValues[1] == @genericArg);
                    });

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_invoke_static_method_with_value_and_generic_with_value_return_type()
        {
            if (DebuggingInterceptorMethod != null) return;

            var @genericArg = new MethodParameterClass();

            var info =
                new MethodInspectorTestMethodGenericInfo(
                    methodName: "StaticMethodWithGenericAndValueTypeArgsAndValueReturnType",
                    methodArgs: new object[] { 1, genericArg },
                    genericTypes: new[] { typeof(MethodParameterClass) },
                    invocation: (invocation) =>
                    {
                        Assert.That((int)invocation.ParameterValues[0] == 1);
                        Assert.That((MethodParameterClass)invocation.ParameterValues[1] == @genericArg);
                    },
                    assertion: (result) =>
                                   {
                                       Assert.That((int)result == 1);
                                   });

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_invoke_static_method_with_value_and_generic_with_generic_return_type()
        {
            if (DebuggingInterceptorMethod != null) return;

            var @genericArg = new MethodParameterClass();

            var info =
                new MethodInspectorTestMethodGenericInfo(
                    methodName: "StaticMethodWithGenericAndValueTypeArgsAndGenericReturnType",
                    methodArgs: new object[] { 1, genericArg },
                    genericTypes: new[] { typeof(MethodParameterClass) },
                    invocation: (invocation) =>
                    {
                        Assert.That((int)invocation.ParameterValues[0] == 1);
                        Assert.That((MethodParameterClass)invocation.ParameterValues[1] == @genericArg);
                    },
                    assertion: (result) =>
                    {
                        Assert.That((MethodParameterClass)result == @genericArg);
                    });

            InterceptedAssembly.AssertResultsFor(info);
        }
    }
}