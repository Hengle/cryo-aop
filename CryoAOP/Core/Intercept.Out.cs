using System;

namespace CryoAOP.Core
{
    public partial class Intercept
    {
        public static event Action<Invocation> Call;

        public static void HandleInvocation(Invocation invocation)
        {
            if (Call != null)
                Call(invocation);
        }

        public static void Clear()
        {
            Call = null;
        }
    }
}