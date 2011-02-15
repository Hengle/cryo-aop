using System;
using System.Linq;
using System.Reflection;
using CryoAOP.Core.Exceptions;
using CryoAOP.Core.Extensions;
using Mono.Cecil;

namespace CryoAOP.Core
{
    public class TypeInspector
    {
        public readonly TypeDefinition Definition;
        public readonly AssemblyInspector AssemblyInspector;

        public TypeInspector(AssemblyInspector assemblyInspector, TypeDefinition definition)
        {
            AssemblyInspector = assemblyInspector;
            Definition = definition;
        }

        public virtual void InterceptAllMethods()
        {
            var methodPrefix = "_{0}_".FormatWith(Guid.NewGuid().ToString("N"));
            foreach (var method in Definition.Methods.ToList())
            {
                var methodInspector = FindMethod(method.Name);
                if (!methodInspector.Definition.IsConstructor)
                    methodInspector.InterceptMethod(methodPrefix);
            }
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
                        return new MethodInspector(this, method);
            }
            throw new MethodNotFoundException("Could not find method '{0}' in type '{1}'", methodName, Definition.Name);
        }

        public override string ToString()
        {
            return "{0}".FormatWith(Definition.FullName);
        }
    }
}