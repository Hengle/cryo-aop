using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CryoAOP.Core.Extensions
{
    public static class ILProcessorExtensions
    {
        public static void Ret(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ret);
            processor.Append(instruction);
        }

        public static void Ldnull(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldnull);
            processor.Append(instruction);
        }

        public static void Call(this MethodDefinition method, MethodReference targetMethod)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Call, targetMethod);
            processor.Append(instruction);
        }

        public static void Callvirt(this MethodDefinition method, MethodReference targetMethod)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Callvirt, targetMethod);
            processor.Append(instruction);
        }

        public static void Nop(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Nop);
            processor.Append(instruction);
        }

        public static void Ldtoken(this MethodDefinition method, TypeReference typeReference)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldtoken, typeReference);
            processor.Append(instruction);
        }

        public static void Ldtoken(this MethodDefinition method, MethodReference methodReference)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldtoken, methodReference);
            processor.Append(instruction);
        }

        public static void Ldstr(this MethodDefinition method, string value)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldstr, value);
            processor.Append(instruction);
        }

        public static void Newarr(this MethodDefinition method, TypeReference type)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Newarr, type);
            processor.Append(instruction);
        }

        public static void NewObj(this MethodDefinition method, MethodReference constructor)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Newobj, constructor);
            processor.Append(instruction);
        }

        public static void Stloc_0(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Stloc_0);
            processor.Append(instruction);
        }

        public static void Stloc_1(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Stloc_1);
            processor.Append(instruction);
        }

        public static void Stloc_2(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Stloc_2);
            processor.Append(instruction);
        }

        public static void Stloc_3(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Stloc_3);
            processor.Append(instruction);
        }

        public static void Ldloc_0(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldloc_0);
            processor.Append(instruction);
        }

        public static void Ldloc_1(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldloc_1);
            processor.Append(instruction);
        }

        public static void Ldloc_2(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldloc_2);
            processor.Append(instruction);
        }

        public static void Ldloc_3(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldloc_3);
            processor.Append(instruction);
        }

        public static void Stelem_ref(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Stelem_Ref);
            processor.Append(instruction);
        }

        public static void Ldarg_0(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldarg_0);
            processor.Append(instruction);
        }

        public static void Ldarg_1(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldarg_1);
            processor.Append(instruction);
        }

        public static void Ldarg_2(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldarg_2);
            processor.Append(instruction);
        }

        public static void Ldarg_3(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldarg_3);
            processor.Append(instruction);
        }

        public static void Ldarg(this MethodDefinition method, ushort index)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldarg, index);
            processor.Append(instruction);
        }

        public static void Box(this MethodDefinition method, TypeReference valueType)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Box, valueType);
            processor.Append(instruction);
        }

        public static void Ldc_I4(this MethodDefinition method, int value)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldc_I4, value);
            processor.Append(instruction);
        }

        public static void Ldc_I4_S(this MethodDefinition method, sbyte value)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldc_I4_S, value);
            processor.Append(instruction);
        }

        public static void Ldc_I4_0(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldc_I4_0);
            processor.Append(instruction);
        }

        public static void Ldc_I4_1(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldc_I4_1);
            processor.Append(instruction);
        }

        public static void Ldc_I4_2(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldc_I4_2);
            processor.Append(instruction);
        }

        public static void Ldc_I4_3(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldc_I4_3);
            processor.Append(instruction);
        }

        public static void Ldc_I4_4(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldc_I4_4);
            processor.Append(instruction);
        }

        public static void Ldc_I4_5(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldc_I4_5);
            processor.Append(instruction);
        }

        public static void Ldc_I4_6(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldc_I4_6);
            processor.Append(instruction);
        }

        public static void Ldc_I4_7(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldc_I4_7);
            processor.Append(instruction);
        }

        public static void Ldc_I4_8(this MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            var instruction = processor.Create(OpCodes.Ldc_I4_8);
            processor.Append(instruction);
        }
    }
}