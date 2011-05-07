using System.Collections.Generic;
using CryoAOP.Core.Cache;
using Mono.Cecil;

namespace CryoAOP.Core.Factories
{
    internal class AssemblyFactory
    {
        private readonly static IMemoryCacheGeneric cache = new MemoryCacheGeneric();

        public Assembly CreateAssembly(System.Reflection.Assembly assembly)
        {
            var assemblyHash = assembly.FullName;
            if (cache.ContainsKey<Assembly>(assemblyHash))
                return cache.Get<Assembly>(assemblyHash);

            var a = new Assembly(assembly);
            cache.Set(assemblyHash, a);
            return a;
        }

        public Assembly CreateAssembly(string assemblyPath, ReaderParameters @params = null)
        {
            var assemblyHash = assemblyPath;
            if (cache.ContainsKey<Assembly>(assemblyHash))
                return cache.Get<Assembly>(assemblyHash);

            var a = new Assembly(assemblyPath, @params);
            cache.Set(assemblyHash, a);
            return a;
        }
    }
}