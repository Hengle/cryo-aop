using Mono.Cecil;

namespace CryoAOP.Core
{
    public class MethodInspector
    {
        public readonly MethodDefinition Definition;

        public MethodInspector(MethodDefinition definition)
        {
            Definition = definition;
        }

        public void CloneAndReplaceMethodCall(string methodPrefix)
        {
            
        }
    }
}