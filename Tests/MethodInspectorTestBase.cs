using System.Reflection;
using CryoAOP.Core;
using CryoAOP.TestAssembly;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    [TestFixture, Explicit]
    public class MethodInspectorTestBase
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            GlobalInterceptor.Clear();
        }

        #endregion

        protected static Assembly InterceptedAssembly;


        protected virtual string InputAssembly
        {
            get { return "CryoAOP.TestAssembly.dll"; }
        }

        protected virtual string OutputAssembly
        {
            get { return "CryoAOP.TestAssembly_Intercepted.dll"; }
        }

        protected virtual string InterceptType
        {
            get { return "CryoAOP.TestAssembly.TypeThatShouldBeIntercepted"; }
        }

        protected virtual string DebuggingInterceptorMethod
        {
            get { return null; }
        }

        [TestFixtureSetUp]
        public virtual void FixtureSetUp()
        {
            if (DebuggingInterceptorMethod == null)
                Interception.RegisterType(InputAssembly, InterceptType, OutputAssembly);
            else
                Interception.RegisterType(InputAssembly, InterceptType, DebuggingInterceptorMethod, OutputAssembly);

            InterceptedAssembly = Assembly.LoadFrom(OutputAssembly);
        }

        [Test]
        public void Should_call_generic_method_where_value_type_is_first_parameter()
        {
            if (DebuggingInterceptorMethod != null) return;

            var a = 1;
            var b = new MethodParameterClass();

            var info =
                new MethodInspectorTestMethodGenericInfo(
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
            if (DebuggingInterceptorMethod != null) return;

            var a = 1;
            var b = new MethodParameterClass();

            var info =
                new MethodInspectorTestMethodGenericInfo(
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
        public void Should_call_generic_with_all_kinds_of_parameters_and_return_a_value_type()
        {
            if (DebuggingInterceptorMethod != null) return;

            int a = 1;
            double b = 2;
            var parameterClass = new MethodParameterClass();

            var info =
                new MethodInspectorTestMethodGenericInfo(
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


            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_call_to_generic_method()
        {
            if (DebuggingInterceptorMethod != null) return;

            var info =
                new MethodInspectorTestMethodGenericInfo(
                    methodName: "GenericMethod",
                    genericTypes: new[] { typeof(MethodParameterClass) });
            
            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_call_to_generic_method_with_generic_parameters()
        {
            if (DebuggingInterceptorMethod != null) return;

            var info =
                new MethodInspectorTestMethodGenericInfo(
                    methodName: "GenericMethodWithGenericParameters",
                    genericTypes: new[] { typeof(MethodParameterClass) }, 
                    methodArgs: new[] { new MethodParameterClass() });

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_call_to_generic_method_with_generic_parameters_and_generic_return_type()
        {
            if (DebuggingInterceptorMethod != null) return;

            var parameterClass = new MethodParameterClass();

            var info =
                new MethodInspectorTestMethodGenericInfo(
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


            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_call_to_generic_with_generic_parameters_and_value_types()
        {
            if (DebuggingInterceptorMethod != null) return;

            var a = 1;
            double b = 2;
            var parameterClass = new MethodParameterClass();

            var info =
                new MethodInspectorTestMethodGenericInfo(
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


            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_intercept_method_and_call()
        {
            if (DebuggingInterceptorMethod != null) return;
            
            int a = 1;
            string b = "2";
            double c = 3;

            var info =
                new MethodInspectorTestMethodInfo(
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

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_intercept_method_and_call_using_reflection_changing_return_value()
        {
            if (DebuggingInterceptorMethod != null) return;

            int a = 1;
            string b = "2";
            double c = 3;

            var info =
                new MethodInspectorTestMethodInfo(
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

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_intercept_method_and_call_with_result()
        {
            if (DebuggingInterceptorMethod != null) return;

            int a = 1;
            string b = "2";
            double c = 3;

            var info =
                new MethodInspectorTestMethodInfo(
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

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_intercept_method_and_cancel_invocation_returning_fake_result()
        {
            if (DebuggingInterceptorMethod != null) return;

            int a = 1;
            string b = "2";
            double c = 3;

            var info =
                new MethodInspectorTestMethodInfo(
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

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_intercept_method_with_class_args_and_call()
        {
            if (DebuggingInterceptorMethod != null) return;

            var param = InterceptedAssembly.CreateInstance(typeof (MethodParameterClass).FullName);

            var info =
                new MethodInspectorTestMethodInfo(
                    methodName: "HavingMethodWithClassArgsAndClassReturnType",
                    methodArgs: new[] { param });

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test] 
        public void Should_call_generic_method_with_two_generic_parameters()
        {
            if (DebuggingInterceptorMethod != null) return;

            var a = 1;
            var b = new MethodParameterClass();

            var info =
                new MethodInspectorTestMethodGenericInfo(
                    methodName: "GenericMethodWithTwoGenericParameters",
                    genericTypes: new[] { typeof(int), typeof(MethodParameterClass) },
                    methodArgs: new object[] { a, b },
                    invocation: (invocation) =>
                                    {
                                        Assert.That(a, Is.EqualTo((int)invocation.ParameterValues[0]));
                                        Assert.That(b, Is.EqualTo(invocation.ParameterValues[1]));
                                    },
                    assertion: (result) => { });


            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_invoke_static_method_with_no_params_and_no_return_type()
        {
            if (DebuggingInterceptorMethod != null) return;

            var info =
                new MethodInspectorTestMethodInfo(
                    methodName: "StaticMethodWithNoArgsAndNoReturnType");

            InterceptedAssembly.AssertResultsFor(info);
        }

        [Test]
        public void Should_invoke_static_method_with_some_params_and_no_return_type()
        {
            if (DebuggingInterceptorMethod != null) return;

            var info =
                new MethodInspectorTestMethodInfo(
                    methodName: "StaticMethodWithArgsAndNoReturnType",
                    methodArgs: new object[] { 1 }, 
                    invocation: (invocation) =>
                                    {
                                        Assert.That((int)invocation.ParameterValues[0] == 1);
                                    });

            InterceptedAssembly.AssertResultsFor(info);
        }
    }
}