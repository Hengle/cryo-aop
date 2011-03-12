using System;
using CryoAOP.Core;
using CryoAOP.Core.Attributes;

namespace CryoAOP.TestAssembly
{
    public class MethodInterceptorMixin
    {
        [MixinMethod(typeof(MethodInterceptorTarget))]
        public void WhenMethodCalled(Action<MethodInvocation> i)
        {
            Intercept.Call += i;
        }
        
        [MixinMethod(typeof(MethodInterceptorTarget))]
        public static void WhenStaticMethodCalled(Action<MethodInvocation> i)
        {
            Intercept.Call += i;
        }
    }
}