using System;
using System.Linq;
using CryoAOP.Core.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CryoAOP.Core
{
    public class MethodInspector
    {
        public readonly MethodDefinition Definition;

        public MethodInspector(MethodDefinition definition)
        {
            Definition = definition;
        }

        public void InterceptAndClone(string methodSuffix)
        {
            var originalMethodName = Definition.Name;
            var newMethodName = "{0}_{1}".FormatWith(originalMethodName, methodSuffix);
            var clonedMethodDefinition = new MethodDefinition(newMethodName, Definition.Attributes, Definition.ReturnType);

            if (Definition.HasParameters)
                foreach (var parameter in Definition.Parameters)
                    clonedMethodDefinition.Parameters.Add(parameter);

            CreateInvocationAndIntercept(clonedMethodDefinition);
            CallFromCloneToActual(clonedMethodDefinition, Definition);
            Definition.DeclaringType.Methods.Add(clonedMethodDefinition);
        }

        private void CallFromCloneToActual(MethodDefinition clonedMethodDefinition, MethodDefinition actualMethodDefinition)
        {
            var ilProcessor = clonedMethodDefinition.Body.GetILProcessor();
            NoOp(ilProcessor);
            Call(ilProcessor, actualMethodDefinition);
            Ret(ilProcessor);
        }

        public void CreateInvocationAndIntercept(MethodDefinition clonedMethodDefinition)
        {
            var ilProcessor = clonedMethodDefinition.Body.GetILProcessor();
            NoOp(ilProcessor);
            LdToken(ilProcessor, Definition.DeclaringType);
            LdStr(ilProcessor, clonedMethodDefinition.Name);

            var findAssembly = AssemblyDefinition.ReadAssembly("System.dll");


            TypeDefinition findType = null;
            foreach (var currentModule in findAssembly.Modules)
            {
                findType = currentModule.Types.Where(t => t.FullName == typeof (Type).FullName).FirstOrDefault();
                if (findType != null) break;
            }
            var findMethod = findType.Methods.Where(m => m.Name == "GetMethod").FirstOrDefault();

            var instruction = ilProcessor.Create(OpCodes.Callvirt, findMethod);
            ilProcessor.Append(instruction);
        }

        private void LdStr(ILProcessor ilProcessor, string str)
        {
            var instruction = ilProcessor.Create(OpCodes.Ldstr, str);
            ilProcessor.Append(instruction);
        }

        private void LdToken(ILProcessor ilProcessor, TypeDefinition typeDefinition)
        {
            var instuction = ilProcessor.Create(OpCodes.Ldtoken, typeDefinition);
            ilProcessor.Append(instuction);
        }

        private void Ret(ILProcessor ilProcessor)
        {
            var retInstruction = ilProcessor.Create(OpCodes.Ret);
            ilProcessor.Append(retInstruction);
        }

        private void Call(ILProcessor ilProcessor, MethodDefinition toMethodDefinition)
        {
            var callInstruction = ilProcessor.Create(OpCodes.Call, toMethodDefinition);
            ilProcessor.Append(callInstruction);
        }

        private void NoOp(ILProcessor clonedMethodIlProcessor)
        {
            var noOpInstruction = clonedMethodIlProcessor.Create(OpCodes.Nop);
            clonedMethodIlProcessor.Append(noOpInstruction);
        }
    }
}