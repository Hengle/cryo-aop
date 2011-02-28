using System;
using CryoAOP.Core;
using CryoAOP.Core.Extensions;

namespace CryoAOP.TestAssembly
{
    public class TestMethodInterceptorType
    {
        public void HavingMethodWithNoArgsAndNoReturnType()
        {
            var methodInvocation = new MethodInvocation(typeof (TestMethodInterceptorType), null, null);
            methodInvocation.CancelInvocation();

            if (methodInvocation.CanInvoke)
                HavingMethodWithNoArgsAndInt32ReturnType();
        }

        public int HavingMethodWithNoArgsAndInt32ReturnType()
        {
            return 1;
        }

        public void HavingMethodWithArgsAndNoReturnType(int arg1, string arg2, double arg3)
        {
        }

        public string HavingMethodWithArgsAndStringReturnType(int arg1, string arg2, double arg3)
        {
            var result = "{0}, {1}, {2}".FormatWith(arg1, arg2, arg3);
            return result;
        }

        public void HavingMethodWithClassArgsAndNoReturnType(MethodParameterClass arg1)
        {
            Intercept.HandleInvocation(new MethodInvocation(typeof (TestMethodInterceptorType), null, null));
        }

        public MethodParameterClass HavingMethodWithClassArgsAndClassReturnType(MethodParameterClass arg1)
        {
            return new MethodParameterClass();
        }

        public void GenericMethod<T>()
        {
        }

        public void GenericMethodWithGenericParameters<T>(T t)
        {
        }

        public T GenericMethodWithGenericParametersAndGenericReturnType<T>(T t)
        {
            return t;
        }

        public void GenericMethodWithGenericParametersAndValueTypeArgs<T>(T t, int i, double j)
        {
        }

        public double GenericMethodWithGenericParamsAndValueReturnType<T>(T t, int i, double j)
        {
            Console.WriteLine("GenericMethodWithGenericParamsAndValueReturnType(t={0},i={1},j={2})".FormatWith(t, i, j));
            return j;
        }

        public void GenericMethodWithInvertedParams<T>(int i, T t)
        {
        }

        public int GenericMethodWithInvertedParamsAndValueReturnType<T>(int i, T t)
        {
            return i;
        }

        public void GenericMethodWithTwoGenericParameters<I, J>(I i, J j)
        {
        }

        public static void StaticMethodWithNoArgsAndNoReturnType()
        {
        }

        public static void StaticMethodWithArgsAndNoReturnType(int i)
        {
        }

        public static void StaticMethodWithGenericAndValueTypeArgsAndNoReturnType<T>(int i, T t)
        {
        }

        public static int StaticMethodWithGenericAndValueTypeArgsAndValueReturnType<T>(int i, T t)
        {
            return i;
        }

        public static T StaticMethodWithGenericAndValueTypeArgsAndGenericReturnType<T>(int i, T t)
        {
            return t;
        }

    }

    public class MethodParameterClass
    {
        public int Arg1 = -1;
    }
}