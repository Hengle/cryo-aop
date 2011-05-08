using System;
using System.Collections.Generic;

namespace CryoAOP.Core.Attributes
{
    public class AttributeCache<T> where T : Attribute
    {
        internal readonly List<AttributeResult<T>> Attributes;
        internal readonly ShadowAssemblyType ShadowAssembly;

        public AttributeCache(ShadowAssemblyType shadowAssembly, List<AttributeResult<T>> attributes)
        {
            ShadowAssembly = shadowAssembly;
            Attributes = attributes;
        }

        public bool Equals(AttributeCache<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.ShadowAssembly.ShadowAssembly.FullName, ShadowAssembly.ShadowAssembly.FullName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (AttributeCache<T>)) return false;
            return Equals((AttributeCache<T>) obj);
        }

        public override int GetHashCode()
        {
            return ShadowAssembly.ShadowAssembly.FullName.GetHashCode();
        }
    }
}