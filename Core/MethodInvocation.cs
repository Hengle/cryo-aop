using System;
using System.Reflection;

namespace CryoAOP.Core
{
    public class MethodInvocation : Invocation
    {
        private readonly object[] parameterValues;

        public MethodInvocation(Type type, MethodInfo method, params object[] parameterValues)
        {
            this.parameterValues = parameterValues;
            Type = type;
            Method = method;
        }

        public Type Type { private set; get; }
        public MethodInfo Method { private set; get; }

        public ParameterInfo[] Parameters
        {
            get { return Method.GetParameters(); }
        }

        public object[] ParameterValues
        {
            get { return parameterValues; }
        }
    }
}