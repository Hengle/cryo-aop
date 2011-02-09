using System;
using System.Reflection;

namespace CryoAOP.Core
{
    public class Invocation
    {
    }

    public class MethodInvocation : Invocation
    {
        public MethodInvocation(Type type, MethodInfo method, ParameterInfo[] parameters)
        {
            Type = type;
            Method = method;
            Parameters = parameters;
        }

        public Type Type { private set; get; }
        public MethodInfo Method { private set; get; }
        public ParameterInfo[] Parameters { private set; get; }
    }

    public class PropertyInvocation : Invocation
    {
        public PropertyInvocation(PropertyInfo property)
        {
            Property = property;
        }

        public PropertyInfo Property { private set; get; }
    }

    public class FieldInvocation : Invocation
    {
        public FieldInvocation(FieldInfo field)
        {
            Field = field;
        }

        public FieldInfo Field { private set; get; }
    }
}