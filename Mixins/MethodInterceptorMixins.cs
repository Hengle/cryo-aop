using System;
using CryoAOP.Aspects;
using CryoAOP.Core;

namespace CryoAOP.Mixins
{
    public class MethodInterceptorMixins
    {
        [MixinMethod()]
        public void WhenCalled<T>(Action<MethodInvocation> invocation)
        {
            Intercept.Clear();
            Intercept.Call +=
                (i) =>
                    {
                        if (i.Type == typeof(T))
                            invocation(i);
                    };
        }

        [MixinMethod()]
        public static void WhenCalledStatic<T>(Action<MethodInvocation> invocation)
        {
            Intercept.Clear();
            Intercept.Call +=
                (i) =>
                {
                    if (i.Type == typeof(T))
                        invocation(i);
                };
        }
    }
}
