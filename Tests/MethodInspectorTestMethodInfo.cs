using System;
using CryoAOP.Core;

namespace CryoAOP.Tests
{
    public class MethodInspectorTestMethodInfo
    {
        private readonly Type type;
        private readonly string methodName;
        private readonly object[] methodArgs;
        private readonly Action<MethodInvocation> invocation;
        private readonly Action<object> assertion;

        public MethodInspectorTestMethodInfo(Type type, string methodName, object[] methodArgs = null, Action<MethodInvocation> invocation = null, Action<object> assertion = null)
        {
            this.type = type;
            this.methodName = methodName;
            this.methodArgs = methodArgs;
            this.invocation = invocation;
            this.assertion = assertion;
        }

        public Type Type
        {
            get { return type; }
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