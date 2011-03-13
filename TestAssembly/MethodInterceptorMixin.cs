using System;
using CryoAOP.Core;
using CryoAOP.Core.Attributes;

namespace CryoAOP.TestAssembly
{
    public class MethodInterceptorMixin
    {
        [MixinMethod(typeof(MethodInterceptorTarget))]
        public void WhenMethodCalled(Action<MethodInvocation> invocation)
        {
            Intercept.Call +=
                (i) =>
                    {
                        if (i.Type == typeof(MethodInterceptorTarget))
                            invocation(i);
                    };
        }

        [MixinMethod(typeof(MethodInterceptorTarget))]
        public static void WhenStaticMethodCalled(Action<MethodInvocation> invocation)
        {
            Intercept.Call +=
                (i) =>
                {
                    if (i.Type == typeof(MethodInterceptorTarget))
                        invocation(i);
                };
        }

        [MixinMethod(typeof(MethodParameterClass))]
        public static void Foo()
        {
            
        }
    }
}