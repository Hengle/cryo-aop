using System;
using System.IO;
using CryoAOP.Core.Extensions;
using Mono.Cecil;

namespace CryoAOP.Core
{
    public class AssemblyInspector
    {
        private readonly string assemblyPath = "";
        private readonly AssemblyDefinition definition;

        public AssemblyInspector(string assemblyPath)
        {
            if (assemblyPath == null) throw new ArgumentNullException("assemblyPath");
            this.assemblyPath = assemblyPath;
            try
            {
                definition = AssemblyDefinition.ReadAssembly(assemblyPath);
            }
            catch (Exception err)
            {
                throw new AssemblyInspectorAssemblyNotFoundException("Could not load assembly from '{0}'", err, assemblyPath);
            }
        }

        public virtual TypeDefinition FindType(Type searchType)
        {
            foreach (var module in definition.Modules)
            {
                foreach (var type in module.Types)
                {
                    if (type.FullName == searchType.FullName)
                        return type;
                }
            }

            throw new AssemblyInspectorTypeNotFoundException(
                "Could not find type for '{0}' in assembly '{1}'", 
                searchType.FullName, Path.GetFileName(assemblyPath));
        }
    }

    #region Exceptions

    public class AssemblyInspectorException : Exception
    {
        public AssemblyInspectorException(string messageFormat, params object[] args)
            : base(messageFormat.FormatWith(args))
        {
        }
        
        public AssemblyInspectorException(string messageFormat, Exception innerException, params object[] args)
            : base(messageFormat.FormatWith(args), innerException)
        {
        }
    }

    public class AssemblyInspectorAssemblyNotFoundException : AssemblyInspectorException
    {
        public AssemblyInspectorAssemblyNotFoundException(string messageFormat, params object[] args) : base(messageFormat, args)
        {
        }

        public AssemblyInspectorAssemblyNotFoundException(string messageFormat, Exception innerException, params object[] args) : base(messageFormat, innerException, args)
        {
        }
    }

    public class AssemblyInspectorTypeNotFoundException : AssemblyInspectorException
    {
        public AssemblyInspectorTypeNotFoundException(string messageFormat, params object[] args) 
            : base(messageFormat, args)
        {
        }

        public AssemblyInspectorTypeNotFoundException(string messageFormat, Exception innerException, params object[] args) 
            : base(messageFormat, innerException, args)
        {
        }
    }

    #endregion 
}