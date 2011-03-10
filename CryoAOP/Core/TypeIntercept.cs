using System;
using System.Linq;
using System.Reflection;
using CryoAOP.Core.Exceptions;
using CryoAOP.Core.Extensions;
using Mono.Cecil;

namespace CryoAOP.Core
{
    internal class TypeIntercept
    {
        public readonly TypeDefinition Definition;
        public readonly AssemblyIntercept AssemblyIntercept;

        public TypeIntercept(AssemblyIntercept assemblyIntercept, TypeDefinition definition)
        {
            AssemblyIntercept = assemblyIntercept;
            Definition = definition;
        }

        public virtual void InterceptAll(MethodInterceptionScope interceptionScope = MethodInterceptionScope.Shallow)
        {
            foreach (var method in Definition.Methods.ToList())
            {
                var methodInspector = FindMethod(method.Name);
                if (!methodInspector.Definition.IsConstructor)
                    methodInspector.InterceptMethod(interceptionScope);
            }
        }

        public virtual MethodIntercept FindMethod(MethodInfo searchMethod)
        {
            var methodName = searchMethod.Name;
            return FindMethod(methodName);
        }

        public virtual MethodIntercept FindMethod(string methodName)
        {
            if (Definition.HasMethods)
            {
                foreach (var method in Definition.Methods) 
                    if (method.Name == methodName)
                        return new MethodIntercept(this, method);
            }
            throw new MethodNotFoundException("Could not find method '{0}' in type '{1}'", methodName, Definition.Name);
        }

        public override string ToString()
        {
            return "{0}".FormatWith(Definition.FullName);
        }
    }
}