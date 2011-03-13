using System;
using CryoAOP.Core;
using CryoAOP.Core.Attributes;

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
