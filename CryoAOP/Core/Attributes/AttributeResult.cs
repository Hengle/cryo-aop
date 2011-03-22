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

using System;
using System.Reflection;
using CryoAOP.Core.Exceptions;

namespace CryoAOP.Core.Attributes
{
    internal class AttributeResult<T> where T : Attribute
    {
        public AttributeResult(ShadowAssemblyType shadowAssembly, System.Type type, T attribute)
        {
            Type = type;
            Attribute = attribute;
            ShadowAssembly = shadowAssembly;
        }

        public AttributeResult(ShadowAssemblyType shadowAssembly, System.Type type, MethodInfo method, T attribute)
        {
            Type = type;
            Method = method;
            Attribute = attribute;
            ShadowAssembly = shadowAssembly;
        }

        public AttributeResult(ShadowAssemblyType shadowAssembly, System.Type type, PropertyInfo property, T attribute)
        {
            Type = type;
            Property = property;
            Attribute = attribute;
            ShadowAssembly = shadowAssembly;
        }

        public T Attribute { get; private set; }
        public System.Type Type { get; private set; }
        public MethodInfo Method { get; private set; }
        public PropertyInfo Property { get; private set; }
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

        public string PropertyName
        {
            get
            {
                if (IsForProperty())
                    return Property.Name;
                throw new IncorrectAttributeResultUsageException(
                    "This attribute result applies to types only and cannot be used for properties.");
            }
        }

        public bool IsForType()
        {
            return Type != null && Method == null && Property == null;
        }

        public bool IsForMethod()
        {
            return Method != null;
        }

        public bool IsForProperty()
        {
            return Property != null;
        }
    }
}