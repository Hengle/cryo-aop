using System;
using CryoAOP.Core;

namespace CryoAOP.Tests
{
    public class MethodInspectorTestMethodInfo
    {
        private readonly string methodName;
        private readonly object[] methodArgs;
        private readonly Action<MethodInvocation> invocation;
        private readonly Action<object> assertion;

        public MethodInspectorTestMethodInfo(string methodName, object[] methodArgs = null, Action<MethodInvocation> invocation = null, Action<object> assertion = null)
        {
            this.methodName = methodName;
            this.methodArgs = methodArgs;
            this.invocation = invocation;
            this.assertion = assertion;
        }

        public string MethodName
        {
            get { return methodName; }
        }
        public object[] MethodArgs
        {
            get { return methodArgs; }
        }
        public Action<MethodInvocation> Invocation
        {
            get { return invocation; }
        }
        public Action<object> Assertion
        {
            get { return assertion; }
        }
    }
}