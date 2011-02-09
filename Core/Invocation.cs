using System.Reflection;

namespace CryoAOP.Core
{
    public class Invocation
    {
    }

    public class MethodInvocation : Invocation
    {
        public MethodInfo Method { private set; get; }

        public MethodInvocation(MethodInfo method)
        {
            Method = method;
        }
    }

    public class PropertyInvocation : Invocation
    {
        public PropertyInfo Property { private set; get; }

        public PropertyInvocation(PropertyInfo property)
        {
            Property = property;
        }
    }

    public class FieldInvocation : Invocation
    {
        public FieldInfo Field { private set; get; }

        public FieldInvocation(FieldInfo field)
        {
            Field = field;
        }
    }
}