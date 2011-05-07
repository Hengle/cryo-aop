using System.Linq;
using System.Reflection;
using CryoAOP.Core.Exceptions;
using CryoAOP.Core.Extensions;
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

        public virtual void InterceptAll(
            MethodInterceptionScopeType interceptionScope = MethodInterceptionScopeType.Shallow)
        {
            foreach (var method in Definition.Methods.ToList())
            {
                var methodInspector = FindMethod(method.Name);
                if (!methodInspector.MethodDefinition.IsConstructor)
                    methodInspector.InterceptMethod(interceptionScope);
            }
            foreach (var property in Definition.Properties.ToList())
            {
                var propertyInspector = FindProperty(property.Name);
                propertyInspector.InterceptProperty(interceptionScope);
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

        public virtual Property FindProperty(string propertyName)
        {
            if (Definition.HasProperties)
            {
                foreach (var property in Definition.Properties)
                    if (property.Name == propertyName)
                        return new Property(this, property);
            }
            throw new PropertyNotFoundException("Could not find method '{0}' in type '{1}'", propertyName,
                                                Definition.Name);
        }

        public override string ToString()
        {
            return "{0}".FormatWith(Definition.FullName);
        }
    }
}