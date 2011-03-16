using System;
using CryoAOP.Aspects;
using CryoAOP.Core;
using CryoAOP.Core.Methods;

namespace CryoAOP.Mixins
{
    public interface IMethodMixins
    {
        void WhenMethodCalled(Action<MethodInvocation> invocation);
    }

    public class MethodMixins
    {
        [MixinMethod()]
        public void WhenCalled<T>(Action<MethodInvocation> invocation)
        {
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
            Intercept.Call +=
                (i) =>
                {
                    if (i.Type == typeof(T))
                        invocation(i);
                };
        }
    }
}
