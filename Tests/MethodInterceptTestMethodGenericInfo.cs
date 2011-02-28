using System;
using CryoAOP.Core;

namespace CryoAOP.Tests
{
    public class MethodInterceptTestMethodGenericInfo : MethodInterceptTestMethodInfo
    {
        private readonly Type[] genericTypes;

        public MethodInterceptTestMethodGenericInfo(Type type, string methodName, Type[] genericTypes, object[] methodArgs = null, Action<MethodInvocation> invocation = null, Action<object> assertion = null)
            : base(type, methodName, methodArgs, invocation, assertion)
        {
            this.genericTypes = genericTypes;
        }

        public Type[] GenericTypes
        {
            get { return genericTypes; }
        }
    }
}