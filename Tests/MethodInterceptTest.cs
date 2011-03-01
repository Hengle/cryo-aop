using System;
using System.Reflection;
using CryoAOP.TestAssembly;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    [TestFixture]
    public class MethodInterceptTest : MethodInterceptTestBase
    {
        [Test]
        public void Should_call_generic_method_where_value_type_is_first_parameter()
        {
            if (IsDebugging) return;

            var a = 1;
            var b = new MethodParameterClass();

            var info =
                new MethodInterceptTestMethodGenericInfo(
                    type: typeof(TestMethodInterceptorType),
                    methodName: "GenericMethodWithInvertedParams",
                    genericTypes: new[] {typeof (MethodParameterClass)},
                    methodArgs: new object[] {1, b},
                    invocation: (invocation) =>
                                    {
                                        Assert.That(a, Is.EqualTo((int) invocation.ParameterValues[0]));
                                        Assert.That(b, Is.EqualTo(invocation.ParameterValues[1]));
                                    },
                    assertion: (result) => { });


            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_call_generic_method_where_value_type_is_first_parameter_and_has_value_return_type()
        {
            if (IsDebugging) return;

            var a = 1;
            var b = new MethodParameterClass();

            var info =
                new MethodInterceptTestMethodGenericInfo(
                    type: typeof(TestMethodInterceptorType),
                    methodName: "GenericMethodWithInvertedParamsAndValueReturnType",
                    genericTypes: new[] {typeof (MethodParameterClass)},
                    methodArgs: new object[] {1, b},
                    invocation: (invocation) =>
                                    {
                                        Assert.That(a, Is.EqualTo((int) invocation.ParameterValues[0]));
                                        Assert.That(b, Is.EqualTo(invocation.ParameterValues[1]));
                                    },
                    assertion: (result) => { Assert.That(result, Is.EqualTo(a)); });


            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_call_generic_method_with_two_generic_parameters()
        {
            if (IsDebugging) return;

            var a = 1;
            var b = new MethodParameterClass();

            var info =
                new MethodInterceptTestMethodGenericInfo(
                    type: typeof(TestMethodInterceptorType),
                    methodName: "GenericMethodWithTwoGenericParameters",
                    genericTypes: new[] {typeof (int), typeof (MethodParameterClass)},
                    methodArgs: new object[] {a, b},
                    invocation: (invocation) =>
                                    {
                                        Assert.That(a, Is.EqualTo((int) invocation.ParameterValues[0]));
                                        Assert.That(b, Is.EqualTo(invocation.ParameterValues[1]));
                                    },
                    assertion: (result) => { });


            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_call_generic_with_all_kinds_of_parameters_and_return_a_value_type()
        {
            if (IsDebugging) return;

            var a = 1;
            double b = 2;
            var parameterClass = new MethodParameterClass();

            var info =
                new MethodInterceptTestMethodGenericInfo(
                    type: typeof(TestMethodInterceptorType),
                    methodName: "GenericMethodWithGenericParamsAndValueReturnType",
                    genericTypes: new[] {typeof (MethodParameterClass)},
                    methodArgs: new object[] {parameterClass, a, b},
                    invocation: (invocation) =>
                                    {
                                        Assert.That(a == (int) invocation.ParameterValues[1]);
                                        Assert.That(b == (double) invocation.ParameterValues[2]);
                                    },
                    assertion: (result) => { Assert.That(result, Is.EqualTo(b)); });


            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_call_to_generic_method()
        {
            if (IsDebugging) return;

            var info =
                new MethodInterceptTestMethodGenericInfo(
                    type: typeof(TestMethodInterceptorType),
                    methodName: "GenericMethod",
                    genericTypes: new[] {typeof (MethodParameterClass)});

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_call_to_generic_method_with_generic_parameters()
        {
            if (IsDebugging) return;

            var info =
                new MethodInterceptTestMethodGenericInfo(
                    type: typeof(TestMethodInterceptorType),
                    methodName: "GenericMethodWithGenericParameters",
                    genericTypes: new[] {typeof (MethodParameterClass)},
                    methodArgs: new[] {new MethodParameterClass()});

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_call_to_generic_method_with_generic_parameters_and_generic_return_type()
        {
            if (IsDebugging) return;

            var parameterClass = new MethodParameterClass();

            var info =
                new MethodInterceptTestMethodGenericInfo(
                    type: typeof(TestMethodInterceptorType),
                    methodName: "GenericMethodWithGenericParametersAndGenericReturnType",
                    genericTypes: new[] {typeof (MethodParameterClass)},
                    methodArgs: new object[] {parameterClass},
                    invocation: (invocation) => { },
                    assertion: (result) => { Assert.That(result, Is.EqualTo(parameterClass)); });


            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_call_to_generic_with_generic_parameters_and_value_types()
        {
            if (IsDebugging) return;

            var a = 1;
            double b = 2;
            var parameterClass = new MethodParameterClass();

            var info =
                new MethodInterceptTestMethodGenericInfo(
                    type: typeof(TestMethodInterceptorType),
                    methodName: "GenericMethodWithGenericParametersAndValueTypeArgs",
                    genericTypes: new[] {typeof (MethodParameterClass)},
                    methodArgs: new object[] {parameterClass, a, b},
                    invocation: (invocation) =>
                                    {
                                        Assert.That(a, Is.EqualTo((int) invocation.ParameterValues[1]));
                                        Assert.That(b, Is.EqualTo((double) invocation.ParameterValues[2]));
                                    },
                    assertion: (result) => { Assert.That(result, Is.Null); });


            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_intercept_method_and_call()
        {
            if (IsDebugging) return;

            var a = 1;
            var b = "2";
            double c = 3;

            var info =
                new MethodInterceptTestMethodInfo(
                    type: typeof(TestMethodInterceptorType),
                    methodName: "HavingMethodWithArgsAndNoReturnType",
                    methodArgs: new object[] {a, b, c},
                    invocation: (invocation) =>
                                    {
                                        Assert.That(a, Is.EqualTo((int) invocation.ParameterValues[0]));
                                        Assert.That(b, Is.EqualTo(invocation.ParameterValues[1]));
                                        Assert.That(c, Is.EqualTo((double) invocation.ParameterValues[2]));
                                    },
                    assertion: (result) => { Assert.That(result, Is.Null); });

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_intercept_method_and_call_using_reflection_changing_return_value()
        {
            if (IsDebugging) return;

            var a = 1;
            var b = "2";
            double c = 3;

            var info =
                new MethodInterceptTestMethodInfo(
                    type: typeof(TestMethodInterceptorType),
                    methodName: "HavingMethodWithArgsAndStringReturnType",
                    methodArgs: new object[] {a, b, c},
                    invocation: (invocation) =>
                                    {
                                        Assert.That(a, Is.EqualTo((int) invocation.ParameterValues[0]));
                                        Assert.That(b, Is.EqualTo(invocation.ParameterValues[1]));
                                        Assert.That(c, Is.EqualTo((double) invocation.ParameterValues[2]));

                                        invocation.Result = "Intercepted Result";
                                    },
                    assertion: (result) => { Assert.That(result, Is.EqualTo("Intercepted Result")); });

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_intercept_method_and_call_with_result()
        {
            if (IsDebugging) return;

            var a = 1;
            var b = "2";
            double c = 3;

            var info =
                new MethodInterceptTestMethodInfo(
                    type: typeof(TestMethodInterceptorType),
                    methodName: "HavingMethodWithArgsAndStringReturnType",
                    methodArgs: new object[] {a, b, c},
                    invocation: (invocation) =>
                                    {
                                        Assert.That(a, Is.EqualTo((int) invocation.ParameterValues[0]));
                                        Assert.That(b, Is.EqualTo(invocation.ParameterValues[1]));
                                        Assert.That(c, Is.EqualTo((double) invocation.ParameterValues[2]));
                                    },
                    assertion: (result) => { Assert.That(result, Is.EqualTo("1, 2, 3")); });

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_intercept_method_and_cancel_invocation_returning_fake_result()
        {
            if (IsDebugging) return;

            var a = 1;
            var b = "2";
            double c = 3;

            var info =
                new MethodInterceptTestMethodInfo(
                    type: typeof(TestMethodInterceptorType),
                    methodName: "HavingMethodWithArgsAndStringReturnType",
                    methodArgs: new object[] {a, b, c},
                    invocation: (invocation) =>
                                    {
                                        Assert.That(a, Is.EqualTo((int) invocation.ParameterValues[0]));
                                        Assert.That(b, Is.EqualTo(invocation.ParameterValues[1]));
                                        Assert.That(c, Is.EqualTo((double) invocation.ParameterValues[2]));

                                        invocation.CancelInvocation();
                                        invocation.Result = "Fake Result";
                                    },
                    assertion: (result) => { Assert.That(result, Is.EqualTo("Fake Result")); });

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_intercept_method_with_class_args_and_call()
        {
            if (IsDebugging) return;

            var param = InterceptedAssembly.CreateInstance(typeof (MethodParameterClass).FullName);

            var info =
                new MethodInterceptTestMethodInfo(
                    type: typeof(TestMethodInterceptorType),
                    methodName: "HavingMethodWithClassArgsAndClassReturnType",
                    methodArgs: new[] {param});

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_invoke_static_method_with_no_params_and_no_return_type()
        {
            if (IsDebugging) return;

            var info =
                new MethodInterceptTestMethodInfo(
                    type: typeof(TestMethodInterceptorType),
                    methodName: "StaticMethodWithNoArgsAndNoReturnType");

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_invoke_static_method_with_some_params_and_no_return_type()
        {
            if (IsDebugging) return;

            var info =
                new MethodInterceptTestMethodInfo(
                    type: typeof(TestMethodInterceptorType),
                    methodName: "StaticMethodWithArgsAndNoReturnType",
                    methodArgs: new object[] {1},
                    invocation: (invocation) => { Assert.That((int) invocation.ParameterValues[0] == 1); });

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_invoke_static_method_with_value_and_generic_with_generic_return_type()
        {
            if (IsDebugging) return;

            var @genericArg = new MethodParameterClass();

            var info =
                new MethodInterceptTestMethodGenericInfo(
                    type: typeof(TestMethodInterceptorType),
                    methodName: "StaticMethodWithGenericAndValueTypeArgsAndGenericReturnType",
                    methodArgs: new object[] {1, genericArg},
                    genericTypes: new[] {typeof (MethodParameterClass)},
                    invocation: (invocation) =>
                                    {
                                        Assert.That((int) invocation.ParameterValues[0] == 1);
                                        Assert.That(invocation.ParameterValues[1] == @genericArg);
                                    },
                    assertion: (result) => { Assert.That(result == @genericArg); });

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_invoke_static_method_with_value_and_generic_with_no_return_type()
        {
            if (IsDebugging) return;

            var @genericArg = new MethodParameterClass();

            var info =
                new MethodInterceptTestMethodGenericInfo(
                    type: typeof(TestMethodInterceptorType),
                    methodName: "StaticMethodWithGenericAndValueTypeArgsAndNoReturnType",
                    methodArgs: new object[] {1, genericArg},
                    genericTypes: new[] {typeof (MethodParameterClass)},
                    invocation: (invocation) =>
                                    {
                                        Assert.That((int) invocation.ParameterValues[0] == 1);
                                        Assert.That(invocation.ParameterValues[1] == @genericArg);
                                    });

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_invoke_static_method_with_value_and_generic_with_value_return_type()
        {
            if (IsDebugging) return;

            var @genericArg = new MethodParameterClass();

            var info =
                new MethodInterceptTestMethodGenericInfo(
                    type: typeof(TestMethodInterceptorType),
                    methodName: "StaticMethodWithGenericAndValueTypeArgsAndValueReturnType",
                    methodArgs: new object[] {1, genericArg},
                    genericTypes: new[] {typeof (MethodParameterClass)},
                    invocation: (invocation) =>
                                    {
                                        Assert.That((int) invocation.ParameterValues[0] == 1);
                                        Assert.That(invocation.ParameterValues[1] == @genericArg);
                                    },
                    assertion: (result) => { Assert.That((int) result == 1); });

            InterceptedAssembly.AssertResultsFor(info);
        }
    }
}