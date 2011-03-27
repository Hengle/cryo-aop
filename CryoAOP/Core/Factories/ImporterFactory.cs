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
using System.Linq;
using CryoAOP.Core.Exceptions;
using Mono.Cecil;

namespace CryoAOP.Core.Factories
{
    internal class ImporterFactory
    {
        private readonly MethodContext context;

        public ImporterFactory(MethodContext context)
        {
            this.context = context;
        }

        public AssemblyDefinition AssemblyDefinition
        {
            get { return context.AssemblyDefinition; }
        }

        public virtual FieldReference Import(FieldReference field)
        {
            return AssemblyDefinition.MainModule.Import(field);
        }

        public virtual TypeReference Import(PropertyReference property)
        {
            return AssemblyDefinition.MainModule.Import(property.PropertyType);
        }

        public virtual MethodReference Import(MethodReference method)
        {
            return AssemblyDefinition.MainModule.Import(method);
        }

        public virtual TypeReference Import(TypeReference type)
        {
            return AssemblyDefinition.MainModule.Import(type);
        }

        public virtual TypeReference Import(TypeReference type, TypeReference ctx)
        {
            return AssemblyDefinition.MainModule.Import(type, ctx);
        }

        public virtual MethodReference Import(System.Type searchType, string methodName)
        {
            var typeReference = Import(searchType);

            foreach (var method in typeReference.Resolve().Methods.ToList())
            {
                var methodParts = methodName.Split(',');
                var searchMethodName = methodParts[0];

                if (method.Name == searchMethodName)
                {
                    if (methodName.Contains(","))
                    {
                        var notableParams =
                            methodParts
                                .Except(new[] {searchMethodName.Trim()})
                                .Select(p => p.Trim());

                        var methodParams =
                            method
                                .Parameters
                                .Select(p => p.ParameterType.Name);

                        if (methodParams.All(mp => notableParams.Contains(mp)))
                        {
                            var methodReference = AssemblyDefinition.MainModule.Import(method);
                            return methodReference;
                        }
                    }
                    else
                    {
                        var methodReference = AssemblyDefinition.MainModule.Import(method);
                        return methodReference;
                    }
                }
            }


            throw new MethodNotFoundException(
                "Could not find method '{0}' on type '{1}'",
                methodName, searchType.Name);
        }

        public virtual TypeReference Import(System.Type searchType)
        {
            var assemblyRef = context.Assemblies.CreateAssembly(searchType.Assembly);
            TypeDefinition type = null;
            foreach (var currentType in assemblyRef.Definition.MainModule.Types.ToList())
            {
                if (searchType.IsArray && currentType.Name == searchType.BaseType.Name)
                {
                    type = currentType;
                    break;
                }

                if (currentType.Name != searchType.Name) continue;
                type = currentType;
                break;
            }

            if (type == null)
                throw new TypeNotFoundException("Could not find type '{0}'", searchType.FullName);

            var typeReference = AssemblyDefinition.MainModule.Import(type);
            return typeReference;
        }
    }
}