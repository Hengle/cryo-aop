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
            GlobalInterceptor.Clear();
        }

        #endregion

        private static Assembly assembly;

        private const string interceptedOutputAssembly = "CryoAOP.TestAssembly_Intercepted.dll";

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            //Interception.RegisterType("CryoAOP.TestAssembly", "CryoAOP.TestAssembly.TypeThatShouldBeIntercepted", "GenericMethodWithGenericParamsAndValueReturnType", interceptedOutputAssembly);
            Interception.RegisterType("CryoAOP.TestAssembly", "CryoAOP.TestAssembly.TypeThatShouldBeIntercepted", interceptedOutputAssembly);
            assembly = Assembly.LoadFrom(interceptedOutputAssembly);
        }

        [Test]
        public void Should_call_generic_method_where_value_type_is_first_parameter()
        {
            var a = 1;
            var b = new MethodParameterClass();

            var info =
                new GenericInfo(
                    methodName: "GenericMethodWithInvertedParams",
                    genericTypes: new[] {typeof (MethodParameterClass)},
                    methodArgs: new object[] {1, b},
                    invocation: (invocation) =>
                                    {
                                        Assert.That(a, Is.EqualTo((int) invocation.ParameterValues[0]));
                                        Assert.That(b, Is.EqualTo(invocation.ParameterValues[1]));
                                    },
                    assertion: (result) => { });


            assembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_call_generic_method_where_value_type_is_first_parameter_and_has_value_return_type()
        {
            var a = 1;
            var b = new MethodParameterClass();

            var info =
                new GenericInfo(
                    methodName: "GenericMethodWithInvertedParamsAndValueReturnType",
                    genericTypes: new[] {typeof (MethodParameterClass)},
                    methodArgs: new object[] {1, b},
                    invocation: (invocation) =>
                                    {
                                        Assert.That(a, Is.EqualTo((int) invocation.ParameterValues[0]));
                                        Assert.That(b, Is.EqualTo(invocation.ParameterValues[1]));
                                    },
                    assertion: (result) => { Assert.That(result, Is.EqualTo(a)); });


            assembly.AssertResultsFor(info);
        }

        [Test, Ignore] // See, PanicStationFixture ... this is possible ... 
        public void Should_call_generic_method_with_two_generic_parameters()
        {
            var i = 1;
            var j = new MethodParameterClass();
            var interceptorCount = 0;
            var methodInfo = assembly.GetGenericMethodInfo("GenericMethodWithTwoGenericParameters", typeof (int), typeof (MethodParameterClass));

            GlobalInterceptor.MethodIntercepter += (invocation) =>
                                                       {
                                                           Assert.That(i == (int) invocation.ParameterValues[0]);
                                                           Assert.That(j == invocation.ParameterValues[1]);

                                                           interceptorCount++;
                                                       };

            methodInfo.AutoInstanceInvoke(i, j);
        }

        [Test]
        public void Should_call_generic_with_all_kinds_of_parameters_and_return_a_value_type()
        {
            int a = 1;
            double b = 2;
            var parameterClass = new MethodParameterClass();

            var info =
                new GenericInfo(
                    methodName: "GenericMethodWithGenericParamsAndValueReturnType",
                    genericTypes: new[] {typeof (MethodParameterClass)},
                    methodArgs: new object[] {parameterClass, a, b},
                    invocation: (invocation) =>
                                    {
                                        Assert.That(a == (int) invocation.ParameterValues[1]);
                                        Assert.That(b == (double) invocation.ParameterValues[2]);
                                    },
                    assertion: (result) =>
                                   {
                                       Assert.That(result, Is.EqualTo(b));
                                   });


            assembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_call_to_generic_method()
        {
            var info =
                new GenericInfo(
                    methodName: "GenericMethod",
                    genericTypes: new[] { typeof(MethodParameterClass) });
            
            assembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_call_to_generic_method_with_generic_parameters()
        {
            var info =
                new GenericInfo(
                    methodName: "GenericMethodWithGenericParameters",
                    genericTypes: new[] { typeof(MethodParameterClass) }, 
                    methodArgs: new[] { new MethodParameterClass() });

            assembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_call_to_generic_method_with_generic_parameters_and_generic_return_type()
        {
            var parameterClass = new MethodParameterClass();

            var info =
                new GenericInfo(
                    methodName: "GenericMethodWithGenericParametersAndGenericReturnType",
                    genericTypes: new[] { typeof(MethodParameterClass) },
                    methodArgs: new object[] { parameterClass },
                    invocation: (invocation) =>
                    {
                    },
                    assertion: (result) =>
                    {
                        Assert.That(result, Is.EqualTo(parameterClass));
                    });


            assembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_call_to_generic_with_generic_parameters_and_value_types()
        {
            var a = 1;
            double b = 2;
            var parameterClass = new MethodParameterClass();

            var info =
                new GenericInfo(
                    methodName: "GenericMethodWithGenericParametersAndValueTypeArgs",
                    genericTypes: new[] { typeof(MethodParameterClass) },
                    methodArgs: new object[] { parameterClass, a, b },
                    invocation: (invocation) =>
                    {
                        Assert.That(a, Is.EqualTo( (int) invocation.ParameterValues[1]));
                        Assert.That(b, Is.EqualTo( (double) invocation.ParameterValues[2]));
                    },
                    assertion: (result) =>
                    {
                        Assert.That(result, Is.Null);
                    });


            assembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_intercept_method_and_call()
        {
            int a = 1;
            string b = "2";
            double c = 3;

            var info =
                new NonGenericInfo(
                    methodName: "HavingMethodWithArgsAndNoReturnType",
                    methodArgs: new object[] { a,b,c },
                    invocation: (invocation) =>
                    {
                        Assert.That(a, Is.EqualTo((int)invocation.ParameterValues[0]));
                        Assert.That(b, Is.EqualTo((string)invocation.ParameterValues[1]));
                        Assert.That(c, Is.EqualTo((double)invocation.ParameterValues[2]));
                    },
                    assertion: (result) =>
                    {
                        Assert.That(result, Is.Null);
                    });

            assembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_intercept_method_and_call_using_reflection_changing_return_value()
        {
            int a = 1;
            string b = "2";
            double c = 3;

            var info =
                new NonGenericInfo(
                    methodName: "HavingMethodWithArgsAndStringReturnType",
                    methodArgs: new object[] { a, b, c },
                    invocation: (invocation) =>
                    {
                        Assert.That(a, Is.EqualTo((int)invocation.ParameterValues[0]));
                        Assert.That(b, Is.EqualTo((string)invocation.ParameterValues[1]));
                        Assert.That(c, Is.EqualTo((double)invocation.ParameterValues[2]));
                        
                        invocation.Result = "Intercepted Result";
                    },
                    assertion: (result) =>
                    {
                        Assert.That(result, Is.EqualTo("Intercepted Result"));
                    });

            assembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_intercept_method_and_call_with_result()
        {
            int a = 1;
            string b = "2";
            double c = 3;

            var info =
                new NonGenericInfo(
                    methodName: "HavingMethodWithArgsAndStringReturnType",
                    methodArgs: new object[] { a, b, c },
                    invocation: (invocation) =>
                    {
                        Assert.That(a, Is.EqualTo((int)invocation.ParameterValues[0]));
                        Assert.That(b, Is.EqualTo((string)invocation.ParameterValues[1]));
                        Assert.That(c, Is.EqualTo((double)invocation.ParameterValues[2]));
                    },
                    assertion: (result) =>
                    {
                        Assert.That(result, Is.EqualTo("1, 2, 3"));
                    });

            assembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_intercept_method_and_cancel_invocation_returning_fake_result()
        {
            int a = 1;
            string b = "2";
            double c = 3;

            var info =
                new NonGenericInfo(
                    methodName: "HavingMethodWithArgsAndStringReturnType",
                    methodArgs: new object[] { a, b, c },
                    invocation: (invocation) =>
                    {
                        Assert.That(a, Is.EqualTo((int)invocation.ParameterValues[0]));
                        Assert.That(b, Is.EqualTo((string)invocation.ParameterValues[1]));
                        Assert.That(c, Is.EqualTo((double)invocation.ParameterValues[2]));

                        invocation.CancelInvocation();
                        invocation.Result = "Fake Result";
                    },
                    assertion: (result) =>
                    {
                        Assert.That(result, Is.EqualTo("Fake Result"));
                    });

            assembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_intercept_method_with_class_args_and_call()
        {
            var param = assembly.CreateInstance(typeof (MethodParameterClass).FullName);

            var info =
                new NonGenericInfo(
                    methodName: "HavingMethodWithClassArgsAndClassReturnType",
                    methodArgs: new[] { param });

            assembly.AssertResultsFor(info);
        }
    }
}