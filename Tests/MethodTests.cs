//CryoAOP. Aspect Oriented Framework for .NET.
//Copyright (C) 2011  Gavin van der Merwe (fir3pho3nixx@gmail.com)

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Reflection;
using CryoAOP.Core;
using CryoAOP.Mixins;
using CryoAOP.TestAssembly;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    [TestFixture]
    public class MethodTests
    {
        #region Setup/Teardown

        [SetUp]
        public void Should_clear_intercepts_between_calls()
        {
            Intercept.Clear();
        }

        #endregion

        public MethodInterceptorTarget InterceptInstance = new MethodInterceptorTarget();

        [Test]
        public void Should_call_generic_method_where_value_type_is_first_parameter()
        {
            var a = 1;
            var b = new MethodInterceptorTargetParameter();
            var interceptorWasCalled = false;

            InterceptInstance
                .WhenCalled<MethodInterceptorTarget>(
                    (invocation) =>
                    {
                        interceptorWasCalled = true;
                        Assert.That(a, Is.EqualTo((int)invocation.ParameterValues[0]));
                        Assert.That(b, Is.EqualTo(invocation.ParameterValues[1]));
                    });


            InterceptInstance.GenericMethodWithInvertedParams(1, b);

            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_call_generic_method_where_value_type_is_first_parameter_and_has_value_return_type()
        {
            var a = 1;
            var b = new MethodInterceptorTargetParameter();
            var interceptorWasCalled = false;

            Intercept.Call +=
                (invocation) =>
                    {
                        interceptorWasCalled = true;
                        Assert.That(a, Is.EqualTo((int) invocation.ParameterValues[0]));
                        Assert.That(b, Is.EqualTo(invocation.ParameterValues[1]));
                    };

            InterceptInstance.GenericMethodWithInvertedParamsAndValueReturnType(1, b);
            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_call_generic_method_with_two_generic_parameters()
        {
            var a = 1;
            var b = new MethodInterceptorTargetParameter();
            var interceptorWasCalled = false;

            Intercept.Call +=
                (invocation) =>
                    {
                        interceptorWasCalled = true;
                        Assert.That(a, Is.EqualTo((int) invocation.ParameterValues[0]));
                        Assert.That(b, Is.EqualTo(invocation.ParameterValues[1]));
                    };

            InterceptInstance.GenericMethodWithTwoGenericParameters(1, b);
            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_call_generic_with_all_kinds_of_parameters_and_return_a_value_type()
        {
            var a = 1;
            double b = 2;
            var parameterClass = new MethodInterceptorTargetParameter();
            var interceptorWasCalled = false;

            Intercept.Call +=
                (invocation) =>
                    {
                        interceptorWasCalled = true;
                        Assert.That(a == (int) invocation.ParameterValues[1]);
                        Assert.That(b == (double) invocation.ParameterValues[2]);
                        Assert.That(parameterClass == invocation.ParameterValues[0]);

                        if (invocation.InvocationLifecycle == InvocationLifecycleType.AfterInvocation)
                            Assert.That(invocation.Result, Is.EqualTo(b));
                    };

            InterceptInstance.GenericMethodWithGenericParamsAndValueReturnType(parameterClass, a, b);
            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_call_to_generic_method()
        {
            var interceptorWasCalled = false;

            Intercept.Call +=
                (invocation) => { interceptorWasCalled = true; };

            InterceptInstance.GenericMethod<MethodInterceptorTargetParameter>();
            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_call_to_generic_method_with_generic_parameters()
        {
            var a = new MethodInterceptorTargetParameter();
            var interceptorWasCalled = false;

            Intercept.Call +=
                (invocation) =>
                    {
                        interceptorWasCalled = true;
                        Assert.That(a, Is.EqualTo(invocation.ParameterValues[0]));
                    };

            InterceptInstance.GenericMethodWithGenericParameters(a);
            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_call_to_generic_method_with_generic_parameters_and_generic_return_type()
        {
            var a = new MethodInterceptorTargetParameter();
            var interceptorWasCalled = false;

            Intercept.Call +=
                (invocation) =>
                    {
                        interceptorWasCalled = true;
                        Assert.That(a, Is.EqualTo(invocation.ParameterValues[0]));

                        if (invocation.InvocationLifecycle == InvocationLifecycleType.AfterInvocation)
                            Assert.That(invocation.Result, Is.EqualTo(a));
                    };

            InterceptInstance.GenericMethodWithGenericParametersAndGenericReturnType(a);
            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_call_to_generic_with_generic_parameters_and_value_types()
        {
            var a = 1;
            double b = 2;
            var parameterClass = new MethodInterceptorTargetParameter();
            var interceptorWasCalled = false;

            Intercept.Call +=
                (invocation) =>
                    {
                        interceptorWasCalled = true;
                        Assert.That(a, Is.EqualTo((int) invocation.ParameterValues[1]));
                        Assert.That(b, Is.EqualTo((double) invocation.ParameterValues[2]));
                        Assert.That(parameterClass, Is.EqualTo(invocation.ParameterValues[0]));

                        if (invocation.InvocationLifecycle == InvocationLifecycleType.AfterInvocation)
                            Assert.That(invocation.Result, Is.Null);
                    };

            InterceptInstance.GenericMethodWithGenericParametersAndValueTypeArgs(parameterClass, a, b);
            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_have_instance_when_calling_instance_method()
        {
            var a = 1;
            var b = new MethodInterceptorTargetParameter();
            var interceptorWasCalled = false;

            Intercept.Call +=
                (invocation) =>
                    {
                        interceptorWasCalled = true;
                        Assert.That(invocation.Instance, Is.Not.Null);
                    };

            InterceptInstance.GenericMethodWithInvertedParams(a, b);
            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_intercept_internal_calls_when_doing_deep_interception()
        {
            var interceptorWasCalled = false;

            Intercept.Call +=
                (invocation) => { interceptorWasCalled = true; };

            InterceptInstance.CallToIntercept();
            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_intercept_method_and_call()
        {
            var a = 1;
            var b = "2";
            double c = 3;
            var interceptorWasCalled = false;

            Intercept.Call +=
                (invocation) =>
                    {
                        interceptorWasCalled = true;
                        Assert.That(a, Is.EqualTo((int) invocation.ParameterValues[0]));
                        Assert.That(b, Is.EqualTo(invocation.ParameterValues[1]));
                        Assert.That(c, Is.EqualTo((double) invocation.ParameterValues[2]));

                        if (invocation.InvocationLifecycle == InvocationLifecycleType.AfterInvocation)
                            Assert.That(invocation.Result, Is.Null);
                    };

            InterceptInstance.HavingMethodWithArgsAndNoReturnType(a, b, c);
            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_intercept_method_and_call_using_reflection_changing_return_value()
        {
            var a = 1;
            var b = "2";
            double c = 3;
            var interceptorWasCalled = false;

            Intercept.Call +=
                (invocation) =>
                    {
                        interceptorWasCalled = true;
                        Assert.That(a, Is.EqualTo((int) invocation.ParameterValues[0]));
                        Assert.That(b, Is.EqualTo(invocation.ParameterValues[1]));
                        Assert.That(c, Is.EqualTo((double) invocation.ParameterValues[2]));

                        if (invocation.InvocationLifecycle == InvocationLifecycleType.AfterInvocation)
                            invocation.Result = "Intercepted Result";
                    };

            var result = InterceptInstance.HavingMethodWithArgsAndStringReturnType(a, b, c);
            Assert.That(result, Is.EqualTo("Intercepted Result"));
            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_intercept_method_and_call_with_result()
        {
            var a = 1;
            var b = "2";
            double c = 3;
            var interceptorWasCalled = false;

            Intercept.Call +=
                (invocation) =>
                    {
                        interceptorWasCalled = true;
                        Assert.That(a, Is.EqualTo((int) invocation.ParameterValues[0]));
                        Assert.That(b, Is.EqualTo(invocation.ParameterValues[1]));
                        Assert.That(c, Is.EqualTo((double) invocation.ParameterValues[2]));
                    };

            var result = InterceptInstance.HavingMethodWithArgsAndStringReturnType(a, b, c);
            Assert.That(result, Is.EqualTo("1, 2, 3"));
            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_intercept_method_and_cancel_invocation_returning_fake_result()
        {
            var a = 1;
            var b = "2";
            double c = 3;
            var interceptorWasCalled = false;

            Intercept.Call +=
                (invocation) =>
                    {
                        interceptorWasCalled = true;
                        Assert.That(a, Is.EqualTo((int) invocation.ParameterValues[0]));
                        Assert.That(b, Is.EqualTo(invocation.ParameterValues[1]));
                        Assert.That(c, Is.EqualTo((double) invocation.ParameterValues[2]));

                        invocation.CancelInvocation();
                        invocation.Result = "Fake Result";
                    };

            var result = InterceptInstance.HavingMethodWithArgsAndStringReturnType(a, b, c);
            Assert.That(result, Is.EqualTo("Fake Result"));
            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_intercept_method_with_class_args_and_call()
        {
            var a = new MethodInterceptorTargetParameter();
            var interceptorWasCalled = false;

            Intercept.Call +=
                (invocation) => { interceptorWasCalled = true; };

            InterceptInstance.HavingMethodWithClassArgsAndClassReturnType(a);
            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_invoke_static_method_with_no_params_and_no_return_type()
        {
            var interceptorWasCalled = false;

            Intercept.Call +=
                (invocation) => { interceptorWasCalled = true; };

            MethodInterceptorTarget.StaticMethodWithNoArgsAndNoReturnType();
            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_invoke_static_method_with_some_params_and_no_return_type()
        {
            var a = 1;
            var interceptorWasCalled = false;

            MethodInterceptorTarget.WhenCalledStatic<MethodInterceptorTarget>(
                (invocation) =>
                    {
                        interceptorWasCalled = true;
                        Assert.That((int) invocation.ParameterValues[0] == 1);
                    });

            MethodInterceptorTarget.StaticMethodWithArgsAndNoReturnType(a);
            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_invoke_static_method_with_value_and_generic_with_generic_return_type()
        {
            var a = new MethodInterceptorTargetParameter();
            var interceptorWasCalled = false;

            Intercept.Call +=
                (invocation) =>
                    {
                        interceptorWasCalled = true;
                        Assert.That((int) invocation.ParameterValues[0] == 1);
                        Assert.That(invocation.ParameterValues[1] == a);
                    };

            var result = MethodInterceptorTarget.StaticMethodWithGenericAndValueTypeArgsAndGenericReturnType(1, a);
            Assert.That(result == a);
            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_invoke_static_method_with_value_and_generic_with_no_return_type()
        {
            var a = new MethodInterceptorTargetParameter();
            var interceptorWasCalled = false;

            Intercept.Call +=
                (invocation) =>
                    {
                        interceptorWasCalled = true;
                        Assert.That((int) invocation.ParameterValues[0] == 1);
                        Assert.That(invocation.ParameterValues[1] == a);
                    };

            MethodInterceptorTarget.StaticMethodWithGenericAndValueTypeArgsAndNoReturnType(1, a);
            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_invoke_static_method_with_value_and_generic_with_value_return_type()
        {
            var a = new MethodInterceptorTargetParameter();
            var interceptorWasCalled = false;

            Intercept.Call +=
                (invocation) =>
                    {
                        interceptorWasCalled = true;
                        Assert.That((int) invocation.ParameterValues[0] == 1);
                        Assert.That(invocation.ParameterValues[1] == a);
                    };

            var result = MethodInterceptorTarget.StaticMethodWithGenericAndValueTypeArgsAndValueReturnType(1, a);
            Assert.That(result == 1);
            Assert.That(interceptorWasCalled);
        }

        [Test]
        public void Should_not_give_null_method_info_when_intercepting_private_methods()
        {
            var a = 1;
            var b = 2;
            var c = "Foo";
            var interceptorWasCalled = false;

            Intercept.Call +=
                (invocation) =>
                    {
                        interceptorWasCalled = true;
                        Assert.That(invocation.Method, Is.Not.Null);
                    };

            var method = InterceptInstance
                .GetType()
                .GetMethod(
                    "privateMethodThatBreaksReflectionWhenTryingToGetMethodInfoUsingGetMethod",
                    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            method.Invoke(InterceptInstance, new object[] {a, b, c});
            Assert.That(interceptorWasCalled);
        }
    }
}