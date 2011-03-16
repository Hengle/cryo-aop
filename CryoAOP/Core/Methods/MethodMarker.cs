using System.Linq;
using CryoAOP.Core.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CryoAOP.Core.Methods
{
    public class MethodMarker
    {
        public virtual bool HasMarker(MethodDefinition method, string markerDefinition)
        {
            if (method == null
                || method.Body == null
                || method.Body.Instructions == null
                || method.Body.Instructions.Count <= 2)
                return false;

            var interceptMarker = method.Body.Instructions.ToList().Take(2).ToArray();
            var firstInstruction = interceptMarker.First();
            return
                firstInstruction.OpCode == OpCodes.Ldstr
                && (firstInstruction.Operand as string) == markerDefinition;
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