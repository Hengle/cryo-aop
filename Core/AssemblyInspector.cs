using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CryoAOP.Core.Exceptions;
using CryoAOP.Core.Extensions;
using Mono.Cecil;

namespace CryoAOP.Core
{
    public class AssemblyInspector
    {
        public readonly AssemblyDefinition Definition;
        private readonly string assemblyPath = "";

        public AssemblyInspector(Assembly assembly)
            : this(assembly
                       .CodeBase
                       .Replace("file:///", "")
                       .Replace("/", @"\"))
        {
        }

        public AssemblyInspector(string assemblyPath)
        {
            if (assemblyPath == null) throw new ArgumentNullException("assemblyPath");
            this.assemblyPath =
                !assemblyPath.ToLower().EndsWith(".dll")
                    ? "{0}.dll".FormatWith(assemblyPath)
                    : assemblyPath;

            try
            {
                Definition = AssemblyDefinition.ReadAssembly(this.assemblyPath);
            }
            catch (Exception err)
            {
                throw new AssemblyNotFoundException(
                    "Could not load assembly from '{0}'", err, assemblyPath);
            }
        }

        public virtual TypeInspector FindType(string searchType)
        {
            foreach (var module in Definition.Modules)
            {
                foreach (var type in module.Types)
                {
                    if (type.FullName.ToLower().EndsWith(searchType.ToLower()))
                        return new TypeInspector(this, type);
                }
            }

            throw new TypeNotFoundException(
                "Could not find type for '{0}' in assembly '{1}'",
                searchType, Path.GetFileName(assemblyPath));
        }

        public virtual TypeInspector FindType(Type searchType)
        {
            return FindType(searchType.FullName);
        }

        public virtual TypeReference Import(Type searchType)
        {
            var assemblyRef = new AssemblyInspector(searchType.Assembly);
            var type = assemblyRef.Definition.MainModule.Types.Where(t => t.Name == searchType.Name).FirstOrDefault();

            if (type == null)
                throw new TypeNotFoundException("Could not find type '{0}'", searchType.FullName);

            var typeReference = Definition.MainModule.Import(type);
            return typeReference;
        }

        public virtual MethodReference Import(MethodDefinition method)
        {
            return Definition.MainModule.Import(method);
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
                                var methodReference = Definition.MainModule.Import(method);
                                return methodReference;
                            }

                        }
                    }
                    else
                    {
                        var methodReference = Definition.MainModule.Import(method);
                        return methodReference;
                    }
                }
            }


            throw new MethodNotFoundException(
                "Could not find method '{0}' on type '{1}'", 
                methodName, searchType.Name);

        }

        public virtual void Write(string path)
        {
            Definition.Write(path);
        }

        public override string ToString()
        {
            return "{0}".FormatWith(Definition.FullName);
        }
    }
}