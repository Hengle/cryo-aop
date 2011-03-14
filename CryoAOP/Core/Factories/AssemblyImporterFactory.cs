﻿using System;
using System.Linq;
using CryoAOP.Core.Exceptions;
using Mono.Cecil;

namespace CryoAOP.Core.Factories
{
    internal class AssemblyImporterFactory
    {
        private readonly MethodInterceptContext context;

        public AssemblyImporterFactory(MethodInterceptContext context)
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

        public virtual MethodReference Import(Type searchType, string methodName)
        {
            var typeReference = Import(searchType);

            foreach (var method in typeReference.Resolve().Methods)
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

        public virtual TypeReference Import(Type searchType)
        {
            var assemblyRef = new AssemblyIntercept(searchType.Assembly);
            TypeDefinition type = null;
            foreach (var currentType in assemblyRef.Definition.MainModule.Types)
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