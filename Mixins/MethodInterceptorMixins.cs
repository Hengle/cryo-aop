using System;
using CryoAOP.Core;
using CryoAOP.Core.Attributes;

namespace CryoAOP.Mixins
{
    public interface IMethodInterceptorMixins
    {
        void WhenMethodCalled(Action<MethodInvocation> invocation);
    }

    public class MethodInterceptorMixins
    {
        [MixinMethod()]
        public void WhenMethodCalled(Action<MethodInvocation> invocation)
        {
            Intercept.Call += invocation;
        }

        [MixinMethod()]
        public static void WhenStaticMethodCalled(Action<MethodInvocation> invocation)
        {
            Intercept.Call += invocation;
        }
    }
}
