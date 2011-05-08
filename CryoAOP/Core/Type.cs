using System;
using System.Linq;
using System.Reflection;
using CryoAOP.Core.Exceptions;
using CryoAOP.Core.Extensions;
using Mono.Cecil;

namespace CryoAOP.Core
{
    public class Type : IEquatable<Type>
    {
        public readonly Assembly Assembly;
        public readonly TypeDefinition Definition;

        public string FullName
        {
            get { return Definition.FullName; }
        }

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

        public bool Equals(Type other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.FullName, FullName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Type)) return false;
            return Equals((Type)obj);
        }

        public override int GetHashCode()
        {
            return (FullName != null ? FullName.GetHashCode() : 0);
        }

        public static bool operator ==(Type left, Type right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Type left, Type right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return "{0}".FormatWith(FullName);
        }
    }
}