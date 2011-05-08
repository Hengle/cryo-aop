using System;
using System.IO;
using System.Runtime.InteropServices;
using CryoAOP.Core.Exceptions;
using CryoAOP.Core.Extensions;
using Mono.Cecil;

namespace CryoAOP.Core
{
    public class Assembly : IEquatable<Assembly>
    {
        private readonly string assemblyPath = "";
        public AssemblyDefinition Definition { get; private set; }
        public string FullName { get { return Definition.FullName; } }
        
        public Assembly(System.Reflection.Assembly assembly)
            : this(assembly
                       .CodeBase
                       .Replace("file:///", "")
                       .Replace("/", @"\"),
                   AssemblyParams
                       .DeferredLoad)
        {
        }

        public Assembly(string assemblyPath, ReaderParameters @params = null)
        {
            if (@params == null)
                @params = AssemblyParams.ReadSymbols;

            if (assemblyPath == null) throw new ArgumentNullException("assemblyPath");
            this.assemblyPath = assemblyPath;

            try
            {
                LoadAssemblyDefinition(@params);

                AssemblyParams
                    .AssemblyResolver
                    .AddSearchDirectory(
                        RuntimeEnvironment
                            .GetRuntimeDirectory());
            }
            catch (Exception err)
            {
                throw new AssemblyNotFoundException(
                    "Could not load assembly from '{0}'", err, assemblyPath);
            }
        }

        public virtual void LoadAssemblyDefinition(ReaderParameters @params)
        {
            Definition =
                AssemblyDefinition
                    .ReadAssembly(
                        this.assemblyPath,
                        @params);
        }

        public virtual Type FindType(string searchType)
        {
            foreach (var module in Definition.Modules)
            {
                foreach (var type in module.Types)
                {
                    if (type.FullName.ToLower().EndsWith(searchType.ToLower()))
                        return new Type(this, type);
                }
            }

            throw new TypeNotFoundException(
                "Could not find type for '{0}' in assembly '{1}'",
                searchType, Path.GetFileName(assemblyPath));
        }

        public virtual Type FindType(System.Type searchType)
        {
            return FindType(searchType.FullName);
        }

        public virtual void Write(string path)
        {
            Definition.Write(path, AssemblyParams.WriterParameters);
        }

        public bool Equals(Assembly other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.FullName, FullName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Assembly)) return false;
            return Equals((Assembly) obj);
        }

        public override int GetHashCode()
        {
            return (FullName != null ? FullName.GetHashCode() : 0);
        }

        public static bool operator ==(Assembly left, Assembly right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Assembly left, Assembly right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return "{0}".FormatWith(FullName);
        }
    }
}