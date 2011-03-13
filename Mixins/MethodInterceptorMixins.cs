using System;
using CryoAOP.Aspects;
using CryoAOP.Core;

namespace CryoAOP.Mixins
{
    public class MethodInterceptorMixins
    {
        [MixinMethod()]
        public void WhenMethodCalled<T>(Action<MethodInvocation> invocation)
        {
            Intercept.Call +=
                (i) =>
                    {
                        if (i.Type == typeof(T))
                            invocation(i);
                    };
        }

        [MixinMethod()]
        public static void WhenStaticMethodCalled<T>(Action<MethodInvocation> invocation)
        {
            Intercept.Call +=
                (i) =>
                {
                    if (i.Type == typeof(T))
                        invocation(i);
                };
        }
    }
}
