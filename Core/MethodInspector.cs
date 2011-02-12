using CryoAOP.Core.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CryoAOP.Core
{
    public class MethodInspector
    {
        public readonly TypeInspector TypeInspector;
        public readonly MethodDefinition Definition;

        public MethodInspector(TypeInspector typeInspector, MethodDefinition definition)
        {
            TypeInspector = typeInspector;
            Definition = definition;
        }

        public void Write(string assemblyPath)
        {
            TypeInspector.AssemblyInspector.Write(assemblyPath);
        }

        public void InterceptMethod(string methodPrefix)
        {
            // Create new Method 
            var clonedMethod = new MethodDefinition(Definition.Name, Definition.Attributes, Definition.ReturnType);
            
            // Rename existing method 
            var renamedMethod = Definition;
            renamedMethod.Name = "{0}{1}".FormatWith(methodPrefix, renamedMethod.Name);

            // Copy variables across 
            if (Definition.Body.HasVariables)
                foreach (var variable in Definition.Body.Variables)
                    clonedMethod.Body.Variables.Add(variable);

            // Insert IL call from clone to renamed method
            var ilProcessor = clonedMethod.Body.GetILProcessor();
            var instruction = ilProcessor.Create(OpCodes.Call, renamedMethod);
            ilProcessor.Append(instruction);

            // Add method to type
            Definition.DeclaringType.Methods.Add(clonedMethod);
        }

        public override string ToString()
        {
            return "{0}".FormatWith(Definition.FullName);
        }
    }
}