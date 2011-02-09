using System.Diagnostics;
using System.Reflection;

namespace CryoAOP.TestAssembly
{
    public class MethodInterceptorCILTemplate
    {
        public void InstanceMethodToBeIntercepted()
        {
            var stackTrace = new StackTrace();
            var stackFrames = stackTrace.GetFrames();
            var currentFrame = stackFrames[0];
            
            var method = (MethodInfo)currentFrame.GetMethod();
            var parameters = method.GetParameters();

        }
    }
}