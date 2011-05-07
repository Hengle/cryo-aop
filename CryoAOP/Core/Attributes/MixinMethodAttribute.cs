using System;
using System.Linq;

namespace CryoAOP.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MixinMethodAttribute : Attribute
    {
        private readonly System.Type[] types;

        public MixinMethodAttribute()
        {
        }

        public MixinMethodAttribute(params System.Type[] types)
        {
            this.types = types;
        }

        public System.Type[] Types
        {
            get { return types; }
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