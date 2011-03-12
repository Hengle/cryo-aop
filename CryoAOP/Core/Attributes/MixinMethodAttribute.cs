using System;
using System.Linq;

namespace CryoAOP.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MixinMethodAttribute : Attribute
    {
        private readonly Type[] types;
        public Type[] Types
        {
            get { return types; }
        }

        public MixinMethodAttribute()
        {
        }

        public MixinMethodAttribute(params Type[] types)
        {
            this.types = types;
        }

        public bool IsTypeSpecific
        {
            get { return types != null; }
        }

        public bool IsForType(string typeName)
        {
            return types.Any(t => t.Name == typeName);
        }
    }
}