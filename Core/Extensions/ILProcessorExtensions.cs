using Mono.Cecil.Cil;

namespace CryoAOP.Core.Extensions
{
    public static class ILProcessorExtensions
    {
        public static void Append(this ILProcessor processor, Instruction[] instructions)
        {
            instructions.ForEach(processor.Append);
        }
    }
}