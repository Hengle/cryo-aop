using System;
using CryoAOP.Core.Methods;

namespace CryoAOP.Core
{
    public partial class Intercept
    {
        public static event Action<MethodInvocation> Call;

        public static void HandleInvocation(Invocation invocation)
        {
            if (invocation is MethodInvocation && Call != null)
            {
                var methodInvocation = (MethodInvocation) invocation;
                Call(methodInvocation);
            }
        }

        public static void Clear()
        {
            Call = null;
        }
    }
}