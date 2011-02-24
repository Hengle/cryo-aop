using System.Linq;
using CryoAOP.Core.Extensions;
using Mono.Cecil;

namespace CryoAOP.Core.Factories
{
    public class MethodCloneFactory
    {
        public MethodDefinition Clone(MethodDefinition sourceMethod)
        {
            var clonedMethod = new MethodDefinition(sourceMethod.Name, sourceMethod.Attributes, sourceMethod.ReturnType);
            sourceMethod.DeclaringType.Methods.Add(clonedMethod);

            CloneMethodProperties(clonedMethod, sourceMethod);
            CloneMethodAttributes(clonedMethod, sourceMethod);
            CloneMethodParameters(sourceMethod, clonedMethod);
            CloneGenericParameters(sourceMethod, clonedMethod);

            return clonedMethod;
        }

        private static void CloneMethodParameters(MethodDefinition sourceMethod, MethodDefinition clonedMethod)
        {
            if (sourceMethod.HasParameters)
                foreach (var parameter in sourceMethod.Parameters.ToList())
                    clonedMethod.Parameters.Add(parameter);
        }

        private static void CloneMethodAttributes(MethodDefinition sourceMethod, MethodDefinition clonedMethod)
        {
            clonedMethod.CallingConvention = sourceMethod.CallingConvention;
            clonedMethod.SemanticsAttributes = sourceMethod.SemanticsAttributes;
            sourceMethod.CustomAttributes.ToList().ForEach(a => clonedMethod.CustomAttributes.Add(a));
            sourceMethod.SecurityDeclarations.ToList().ForEach(s => clonedMethod.SecurityDeclarations.Add(s));
        }

        private static void CloneMethodProperties(MethodDefinition sourceMethod, MethodDefinition clonedMethod)
        {
            clonedMethod.IsAbstract = sourceMethod.IsAbstract;
            clonedMethod.IsAddOn = sourceMethod.IsAddOn;
            clonedMethod.IsAssembly = sourceMethod.IsAssembly;
            clonedMethod.IsCheckAccessOnOverride = sourceMethod.IsCheckAccessOnOverride;
            clonedMethod.IsCompilerControlled = sourceMethod.IsCompilerControlled;
            clonedMethod.IsFamily = sourceMethod.IsFamily;
            clonedMethod.IsFamilyAndAssembly = sourceMethod.IsFamilyAndAssembly;
            clonedMethod.IsFamilyOrAssembly = sourceMethod.IsFamilyOrAssembly;
            clonedMethod.IsFinal = sourceMethod.IsFinal;
            clonedMethod.IsFire = sourceMethod.IsFire;
            clonedMethod.IsForwardRef = sourceMethod.IsForwardRef;
            clonedMethod.IsGetter = sourceMethod.IsGetter;
            clonedMethod.IsHideBySig = sourceMethod.IsHideBySig;
            clonedMethod.IsIL = sourceMethod.IsIL;
            clonedMethod.IsInternalCall = sourceMethod.IsInternalCall;
            clonedMethod.IsManaged = sourceMethod.IsManaged;
            clonedMethod.IsNative = sourceMethod.IsNative;
            clonedMethod.IsNewSlot = sourceMethod.IsNewSlot;
            clonedMethod.IsPInvokeImpl = sourceMethod.IsPInvokeImpl;
            clonedMethod.IsPreserveSig = sourceMethod.IsPreserveSig;
            clonedMethod.IsPrivate = sourceMethod.IsPrivate;
            clonedMethod.IsPublic = sourceMethod.IsPublic;
            clonedMethod.IsRemoveOn = sourceMethod.IsRemoveOn;
            clonedMethod.IsReuseSlot = sourceMethod.IsReuseSlot;
            clonedMethod.IsRuntime = sourceMethod.IsRuntime;
            clonedMethod.IsRuntimeSpecialName = sourceMethod.IsRuntimeSpecialName;
            clonedMethod.IsSetter = sourceMethod.IsSetter;
            clonedMethod.IsSpecialName = sourceMethod.IsSpecialName;
            clonedMethod.IsStatic = sourceMethod.IsStatic;
            clonedMethod.IsSynchronized = sourceMethod.IsSynchronized;
            clonedMethod.IsUnmanaged = sourceMethod.IsUnmanaged;
            clonedMethod.IsUnmanagedExport = sourceMethod.IsUnmanagedExport;
            clonedMethod.IsVirtual = sourceMethod.IsVirtual;
            clonedMethod.NoInlining = sourceMethod.NoInlining;
            clonedMethod.NoOptimization = sourceMethod.NoOptimization;
        }

        private static void CloneGenericParameters(MethodDefinition sourceMethod, MethodDefinition clonedMethod)
        {
            if (sourceMethod.HasGenericParameters)
            {
                foreach (var genericParameter in sourceMethod.GenericParameters.ToList())
                {
                    if (genericParameter != null)
                    {
                        var newGenericParameter = new GenericParameter(genericParameter.Name, clonedMethod);
                        clonedMethod.GenericParameters.Add(newGenericParameter);
                        CloneGenericParameterProperties(genericParameter, newGenericParameter);
                    }
                }
            }
        }

        private static void CloneGenericParameterProperties(GenericParameter genericParameter, GenericParameter newGenericParameter)
        {
            newGenericParameter.Attributes = genericParameter.Attributes;
            genericParameter.Constraints.ForEach(gp => newGenericParameter.Constraints.Add(gp));
            genericParameter.CustomAttributes.ForEach(ca => newGenericParameter.CustomAttributes.Add(ca));
            newGenericParameter.DeclaringType = genericParameter.DeclaringType;
            genericParameter.GenericParameters.ForEach(gp => newGenericParameter.GenericParameters.Add(gp));
            newGenericParameter.HasDefaultConstructorConstraint = genericParameter.HasDefaultConstructorConstraint;
            newGenericParameter.IsContravariant = genericParameter.IsContravariant;
            newGenericParameter.IsCovariant = genericParameter.IsCovariant;
            newGenericParameter.IsNonVariant = genericParameter.IsNonVariant;
        }
    }
}