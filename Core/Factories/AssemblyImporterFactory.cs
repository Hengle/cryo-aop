using System;
using CryoAOP.Core.Exceptions;
using Mono.Cecil;

namespace CryoAOP.Core.Factories
{
    public class AssemblyImporterFactory
    {
        private readonly AssemblyDefinition definition;

        public AssemblyImporterFactory(AssemblyDefinition definition)
        {
            this.definition = definition;
        }

        public AssemblyImporterFactory(MethodInspector inspector)
        {
            this.definition = inspector.TypeInspector.AssemblyInspector.Definition;
        }

        public virtual MethodReference Import(MethodDefinition method)
        {
            return definition.MainModule.Import(method);
        }

        public virtual TypeReference Import(TypeDefinition type)
        {
            return definition.MainModule.Import(type);
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
                        foreach (var paramterType in methodParts)
                        {
                            if (paramterType == searchMethodName)
                                continue;

                            var isMatch = true;
                            foreach (var parameters in method.Parameters)
                            {
                                if (parameters.ParameterType.Name != paramterType)
                                {
                                    isMatch = false;
                                    break;
                                }
                            }

                            if (isMatch)
                            {
                                var methodReference = definition.MainModule.Import(method);
                                return methodReference;
                            }
                        }
                    }
                    else
                    {
                        var methodReference = definition.MainModule.Import(method);
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
            var assemblyRef = new AssemblyInspector(searchType.Assembly);
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

            var typeReference = definition.MainModule.Import(type);
            return typeReference;
        }
    }
}