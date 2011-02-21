using System;
using CryoAOP.Core;
using CryoAOP.Core.Extensions;

namespace CryoAOP.TestAssembly
{
    public class TypeThatShouldBeIntercepted
    {
        public void HavingMethodWithNoArgsAndNoReturnType()
        {
            var methodInvocation = new MethodInvocation(typeof (TypeThatShouldBeIntercepted), null, null);
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
            GlobalInterceptor.HandleInvocation(new MethodInvocation(typeof (TypeThatShouldBeIntercepted), null, null));
        }

        public MethodParameterClass HavingMethodWithClassArgsAndClassReturnType(MethodParameterClass arg1)
        {
            return new MethodParameterClass();
        }

        public void GenericMethod<T>()
        {
            //return default(T);
        }

        public void CallToGenericMethod<T>()
        {
            GenericMethod<T>();
        }
    }

    public class MethodParameterClass
    {
        public int Arg1 = -1;
    }
}