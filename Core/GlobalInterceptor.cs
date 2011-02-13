namespace CryoAOP.Core
{
    public class GlobalInterceptor
    {
        public static void HandleInvocation(Invocation invocation)
        {
            var methodInvocation = (MethodInvocation) invocation;
        }
    }
}