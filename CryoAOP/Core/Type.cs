//CryoAOP. Aspect Oriented Framework for .NET.
//Copyright (C) 2011  Gavin van der Merwe (fir3pho3nixx@gmail.com)

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Linq;
using System.Reflection;
using CryoAOP.Aspects;
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
            throw new PropertyNotFoundException("Could not find method '{0}' in type '{1}'", propertyName, Definition.Name);
        }

        public override string ToString()
        {
            return "{0}".FormatWith(Definition.FullName);
        }
    }
}