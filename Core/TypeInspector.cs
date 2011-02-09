using System.Reflection;
using CryoAOP.Core.Exceptions;
using Mono.Cecil;

namespace CryoAOP.Core
{
    public class TypeInspector
    {
        public readonly TypeDefinition Definition;

        public TypeInspector(TypeDefinition definition)
        {
            Definition = definition;
        }

        public virtual MethodInspector FindMethod(MethodInfo searchMethod)
        {
            var methodName = searchMethod.Name;
            return FindMethod(methodName);
        }

        public virtual MethodInspector FindMethod(string methodName)
        {
            if (Definition.HasMethods)
            {
                foreach (var method in Definition.Methods) 
                    if (method.Name == methodName)
                        return new MethodInspector(method);
            }
            throw new MethodNotFoundException("Could not find method '{0}' in type '{1}'", methodName, Definition.Name);
        }
    }
}