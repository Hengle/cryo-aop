using System.Reflection;

namespace CryoAOP.Core
{
    public class Invocation
    {
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