using System.Diagnostics;
using System.Reflection;
using CryoAOP.Core;

namespace CryoAOP.TestAssembly
{
    public class MethodInterceptorCILTemplate
    {
        public void InstanceMethodToBeIntercepted()
        {
            var stackTrace = new StackTrace();
            var stackFrames = stackTrace.GetFrames();
            if (stackFrames != null)
            {
                var currentFrame = stackFrames[0];
                var method = (MethodInfo)currentFrame.GetMethod();
                var parameters = method.GetParameters();
                var methodInvocation = new MethodInvocation(method.DeclaringType, method, parameters);
                GlobalInterceptor.HandleInvocation(methodInvocation);
            }
        }
    }
}