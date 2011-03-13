using System;
using System.Reflection;
using CryoAOP.Core.Exceptions;

namespace CryoAOP.Core
{
    internal class AttributeResult<T> where T : Attribute
    {
        public AttributeResult(ShadowAssemblyType shadowAssembly, Type type, T attribute)
        {
            Type = type;
            Attribute = attribute;
            ShadowAssembly = shadowAssembly;
        }

        public AttributeResult(ShadowAssemblyType shadowAssembly, Type type, MethodInfo method, T attribute)
        {
            Type = type;
            Method = method;
            Attribute = attribute;
            ShadowAssembly = shadowAssembly;
        }

        public T Attribute { get; private set; }
        public Type Type { get; private set; }
        public MethodInfo Method { get; private set; }
        public ShadowAssemblyType ShadowAssembly { get; private set; }

        public string TypeName
        {
            get
            {
                return Type.FullName;
            }
        }

        public string MethodName
        {
            get
            {
                if (IsForMethod())
                    return Method.Name;
                throw new IncorrectAttributeResultUsageException(
                    "This attribute result applies to types only and cannot be used for methods.");
            }
        }

        public bool IsForType()
        {
            return Type != null && Method == null;
        }

        public bool IsForMethod()
        {
            return Method != null;
        }
    }
}