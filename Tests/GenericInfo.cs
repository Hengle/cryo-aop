using System;
using CryoAOP.Core;

namespace CryoAOP.Tests
{
    public class GenericInfo : NonGenericInfo
    {
        private readonly Type[] genericTypes;

        public GenericInfo(string methodName, Type[] genericTypes, object[] methodArgs = null, Action<MethodInvocation> invocation = null, Action<object> assertion = null)
            : base(methodName, methodArgs, invocation, assertion)
        {
            this.genericTypes = genericTypes;
        }

        public Type[] GenericTypes
        {
            get { return genericTypes; }
        }
    }
}