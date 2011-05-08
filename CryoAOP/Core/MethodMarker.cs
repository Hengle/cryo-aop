using System.Collections.Generic;
using System.Linq;
using CryoAOP.Core.Cache;
using CryoAOP.Core.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CryoAOP.Core
{
    public class MethodMarker
    {
        public static readonly IMemoryCacheGeneric cache = new MemoryCacheGeneric();

        public virtual bool HasMarker(MethodDefinition method, string markerDefinition)
        {
            var methodNameHashCode = method.DeclaringType.FullName + method.Name;
            if (cache.ContainsKey<bool>(methodNameHashCode))
                return cache.Get<bool>(methodNameHashCode);

            if (method == null
                || method.Body == null
                || method.Body.Instructions == null
                || method.Body.Instructions.Count <= 2)
                return false;

            var interceptMarker = method.Body.Instructions.ToList().Take(2).ToArray();
            var firstInstruction = interceptMarker.First();
            var hasMarker =
                firstInstruction.OpCode == OpCodes.Ldstr
                && (firstInstruction.Operand as string) == markerDefinition;

            if (hasMarker)
                cache.Set(methodNameHashCode, hasMarker);

            return hasMarker;
        }

        public virtual void CreateMarker(MethodDefinition method, string markerDefinition)
        {
            var il = method.Body.GetILProcessor();
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldstr, markerDefinition),
                              il.Create(OpCodes.Pop)
                          });
        }
    }
}