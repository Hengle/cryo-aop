using System;
using System.Reflection;

namespace CryoAOP.Core
{
    public enum MethodInvocationType
    {
        BeforeInvocation,
        AfterInvocation
    }

    public class MethodInvocation : Invocation
    {
        private readonly object[] parameterValues;

        public MethodInvocation(Type type, MethodInfo method, params object[] parameterValues)
        {
            Type = type;
            Method = method;
            CanInvoke = true;
            this.parameterValues = parameterValues;
            InvocationType = MethodInvocationType.BeforeInvocation;
        }

        public Type Type { private set; get; }
        public MethodInfo Method { private set; get; }
        public MethodInvocationType InvocationType { private set; get; }
        public bool CanInvoke { private set; get; }

        public ParameterInfo[] Parameters
        {
            get { return Method.GetParameters(); }
        }

        public object[] ParameterValues
        {
            get { return parameterValues; }
        }

        private object result;
        public object Result
        {
            get { return result; }
            set { result = value; }
        }

        public virtual void ContinueInvocation()
        {
            CanInvoke = false;
            InvocationType = MethodInvocationType.AfterInvocation;
        }

        public virtual void CancelInvocation()
        {
            CanInvoke = false;
        }

        public override string ToString()
        {
            return string.Format("InvocationType: {4}, Type: {2}, Method: {3}, CanInvoke: {5}, ParameterValues: {1}, Result: {0}", Result, string.Join(",", parameterValues), Type, Method, InvocationType, CanInvoke);
        }
    }
}