using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CryoAOP.Core;
using CryoAOP.Core.Extensions;
using CryoAOP.TestAssembly;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    [TestFixture]
    public class CILInspectorTests
    {
        [Test]
        public void test()
        {
            var cilType = new MethodInterceptorCILTemplate();
            var assemblyInspector = new AssemblyInspector("CryoAOP.TestAssembly");
            var typeInspector = assemblyInspector.FindType(typeof (MethodInterceptorCILTemplate));
            var methodInspector = typeInspector.FindMethod("InstanceMethodToBeIntercepted");

            // Create cloned method definition
            var clonedMethodDef = 
                new MethodDefinition(
                    "_{0}".FormatWith(methodInspector.Definition.Name), 
                    methodInspector.Definition.Attributes, 
                    methodInspector.Definition.ReturnType);

            // typeInspector.Definition.ClassSize

            // Copy the entire method across
            var clonedILProcessor = clonedMethodDef.Body.GetILProcessor();
            foreach (var instruction in methodInspector.Definition.Body.Instructions.ToList())
            {
                // Should we create new instructions???
                //var clonedInstruction = clonedILProcessor.Create()
                clonedILProcessor.Append(instruction);
            }

            // Remove all the instructions off the existing method
            var interceptedIlProcessor = methodInspector.Definition.Body.GetILProcessor();
            foreach (var instruction in methodInspector.Definition.Body.Instructions.ToList())
                interceptedIlProcessor.Remove(instruction);

            // Replace with call to clone
            var i = interceptedIlProcessor.Create(OpCodes.Call, clonedMethodDef);
            interceptedIlProcessor.Append(i);

            typeInspector.Definition.Methods.Add(clonedMethodDef);
            assemblyInspector.Definition.Write("CryoAOP.TestAssembly_Cloned_Method.dll");

            // Now we are going to check whether the method executes 
            var assembly = Assembly.LoadFrom("CryoAOP.TestAssembly_Cloned_Method.dll");
            var fookedType = assembly.GetTypes().Where(t => t.FullName == typeof (MethodInterceptorCILTemplate).FullName).First();
            var methodInfo = fookedType.GetMethod(methodInspector.Definition.Name);
            methodInfo.Invoke(assembly.CreateInstance(fookedType.FullName), null);
        }
    }
}
