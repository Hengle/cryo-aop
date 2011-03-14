using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using CryoAOP.Core.Extensions;

namespace CryoAOP.Core
{
    public enum MethodInterceptionScopeType
    {
        Deep,
        Shallow
    }

    internal class MethodInterceptScopingExtension : MethodInterceptExtension
    {


        public MethodInterceptScopingExtension(MethodInterceptContext methodIntercept) : base(methodIntercept)
        {
        }

        public void ApplyDeepScope(MethodDefinition renamedMethod, MethodDefinition interceptorMethod, ILProcessor il, MethodInterceptionScopeType interceptionScope)
        {
            if (interceptionScope == MethodInterceptionScopeType.Deep)
            {
                foreach (var module in TypeIntercept.AssemblyIntercept.Definition.Modules)
                {
                    foreach (var type in module.Types.ToList())
                    {
                        if (type.Methods == null || type.Methods.Count == 0) continue;
                        foreach (var method in type.Methods.ToList())
                        {
                            if (Context.MethodMarker.HasMarker(method, MethodIntercept.MethodMarker)) continue;

                            if (method == null
                                || method.Body == null
                                || method.Body.Instructions == null
                                || method.Body.Instructions.Count() == 0)
                                continue;

                            foreach (var instruction in method.Body.Instructions.ToList())
                            {
                                if (instruction.OpCode == OpCodes.Call && instruction.Operand == renamedMethod)
                                {
                                    var processor = method.Body.GetILProcessor();
                                    processor.InsertAfter(instruction, il.Create(OpCodes.Call, interceptorMethod));
                                    processor.Remove(instruction);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
