using System;

namespace CryoAOP.Core
{
    public class GlobalInterceptor
    {
        public static event Action<MethodInvocation> MethodIntercepter;

        public static void HandleInvocation(Invocation invocation)
        {
            if (invocation is MethodInvocation && MethodIntercepter != null)
            {
                var methodInvocation = (MethodInvocation) invocation;
                MethodIntercepter(methodInvocation);
            }
        }

        public static void ClearAll()
        {
            MethodIntercepter = null;
        }
    }
}