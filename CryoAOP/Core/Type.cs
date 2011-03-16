using System.Linq;
using System.Reflection;
using CryoAOP.Aspects;
using CryoAOP.Core.Exceptions;
using CryoAOP.Core.Extensions;
using CryoAOP.Core.Methods;
using Mono.Cecil;

namespace CryoAOP.Core
{
    internal class Type
    {
        public readonly Assembly Assembly;
        public readonly TypeDefinition Definition;

        public Type(Assembly assembly, TypeDefinition definition)
        {
            Assembly = assembly;
            Definition = definition;
        }

        public virtual void InterceptAll(MethodInterceptionScopeType interceptionScope = MethodInterceptionScopeType.Shallow)
        {
            foreach (var method in Definition.Methods.ToList())
            {
                var methodInspector = FindMethod(method.Name);
                if (!methodInspector.Definition.IsConstructor)
                    methodInspector.InterceptMethod(interceptionScope);
            }
        }

        public virtual Method FindMethod(MethodInfo searchMethod)
        {
            var methodName = searchMethod.Name;
            return FindMethod(methodName);
        }

        public virtual Method FindMethod(string methodName)
        {
            if (Definition.HasMethods)
            {
                foreach (var method in Definition.Methods)
                    if (method.Name == methodName)
                        return new Method(this, method);
            }
            throw new MethodNotFoundException("Could not find method '{0}' in type '{1}'", methodName, Definition.Name);
        }

        public override string ToString()
        {
            return "{0}".FormatWith(Definition.FullName);
        }
    }
}