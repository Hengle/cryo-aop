using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CryoAOP.Core
{
    public class Invocation
    {
    }

    public class MethodInvocation : Invocation
    {

        public MethodInvocation(Type type, MethodInfo method, List<object> parameterValues)
        {
            Type = type;
            Method = method;
            Parameters = method.GetParameters().ToList();
            ParameterValues = parameterValues;
        }

        public Type Type { private set; get; }
        public MethodInfo Method { private set; get; }
        public List<ParameterInfo> Parameters { private set; get; }
        public List<object> ParameterValues { private set; get; }
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