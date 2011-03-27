using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace CryoAOP.Core.Factories
{
    internal class AssemblyCacheItem
    {
        internal readonly Assembly Assembly;
        internal readonly string AssemblyPath;

        public AssemblyCacheItem(Assembly assembly)
        {
            Assembly = assembly;
        }

        public AssemblyCacheItem(string assemblyPath)
        {
            AssemblyPath = assemblyPath;
        }

        public bool Equals(AssemblyCacheItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Assembly, Assembly) && Equals(other.AssemblyPath, AssemblyPath);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (AssemblyCacheItem)) return false;
            return Equals((AssemblyCacheItem) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Assembly != null ? Assembly.GetHashCode() : 0)*397) ^ (AssemblyPath != null ? AssemblyPath.GetHashCode() : 0);
            }
        }
    }

    internal class AssemblyCache
    {
        internal static Dictionary<int, Assembly> Cache = new Dictionary<int, Assembly>();
    }

    internal class AssemblyFactory
    {
        public Assembly CreateAssembly(System.Reflection.Assembly assembly)
        {
            int assemblyHash = assembly.FullName.GetHashCode();
            if (AssemblyCache.Cache.ContainsKey(assemblyHash))
                return AssemblyCache.Cache[assemblyHash];

            var a = new Assembly(assembly);
            AssemblyCache.Cache.Add(assemblyHash, a);
            return a;
        }

        public Assembly CreateAssembly(string assemblyPath, ReaderParameters @params = null)
        {
            int assemblyHash = assemblyPath.GetHashCode();
            if (AssemblyCache.Cache.ContainsKey(assemblyHash))
                return AssemblyCache.Cache[assemblyHash];
         
            var a = new Assembly(assemblyPath, @params);
            AssemblyCache.Cache.Add(assemblyHash, a);
            return a;
        }
    }
}
