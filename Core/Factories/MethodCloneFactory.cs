using System.Linq;
using Mono.Cecil;

namespace CryoAOP.Core.Factories
{
    public class MethodCloneFactory
    {
        public MethodDefinition Clone(MethodDefinition sourceMethod)
        {
            var clonedMethod = new MethodDefinition(sourceMethod.Name, sourceMethod.Attributes, sourceMethod.ReturnType);
            sourceMethod.DeclaringType.Methods.Add(clonedMethod);

            // Clone method properties 
            CloneMethodProperties(clonedMethod, sourceMethod);

            // Copy calling convention
            clonedMethod.CallingConvention = sourceMethod.CallingConvention;

            // Copy method semantics 
            clonedMethod.SemanticsAttributes = sourceMethod.SemanticsAttributes;

            // Copy attributes 
            sourceMethod.CustomAttributes.ToList().ForEach(a => clonedMethod.CustomAttributes.Add(a));

            // Copy security declarations 
            sourceMethod.SecurityDeclarations.ToList().ForEach(s => clonedMethod.SecurityDeclarations.Add(s));

            return sourceMethod;
        }

        private static void CloneMethodProperties(MethodDefinition interceptorMethod, MethodDefinition renamedMethod)
        {
            interceptorMethod.IsAbstract = renamedMethod.IsAbstract;
            interceptorMethod.IsAddOn = renamedMethod.IsAddOn;
            interceptorMethod.IsAssembly = renamedMethod.IsAssembly;
            interceptorMethod.IsCheckAccessOnOverride = renamedMethod.IsCheckAccessOnOverride;
            interceptorMethod.IsCompilerControlled = renamedMethod.IsCompilerControlled;
            interceptorMethod.IsFamily = renamedMethod.IsFamily;
            interceptorMethod.IsFamilyAndAssembly = renamedMethod.IsFamilyAndAssembly;
            interceptorMethod.IsFamilyOrAssembly = renamedMethod.IsFamilyOrAssembly;
            interceptorMethod.IsFinal = renamedMethod.IsFinal;
            interceptorMethod.IsFire = renamedMethod.IsFire;
            interceptorMethod.IsForwardRef = renamedMethod.IsForwardRef;
            interceptorMethod.IsGetter = renamedMethod.IsGetter;
            interceptorMethod.IsHideBySig = renamedMethod.IsHideBySig;
            interceptorMethod.IsIL = renamedMethod.IsIL;
            interceptorMethod.IsInternalCall = renamedMethod.IsInternalCall;
            interceptorMethod.IsManaged = renamedMethod.IsManaged;
            interceptorMethod.IsNative = renamedMethod.IsNative;
            interceptorMethod.IsNewSlot = renamedMethod.IsNewSlot;
            interceptorMethod.IsPInvokeImpl = renamedMethod.IsPInvokeImpl;
            interceptorMethod.IsPreserveSig = renamedMethod.IsPreserveSig;
            interceptorMethod.IsPrivate = renamedMethod.IsPrivate;
            interceptorMethod.IsPublic = renamedMethod.IsPublic;
            interceptorMethod.IsRemoveOn = renamedMethod.IsRemoveOn;
            interceptorMethod.IsReuseSlot = renamedMethod.IsReuseSlot;
            interceptorMethod.IsRuntime = renamedMethod.IsRuntime;
            interceptorMethod.IsRuntimeSpecialName = renamedMethod.IsRuntimeSpecialName;
            interceptorMethod.IsSetter = renamedMethod.IsSetter;
            interceptorMethod.IsSpecialName = renamedMethod.IsSpecialName;
            interceptorMethod.IsStatic = renamedMethod.IsStatic;
            interceptorMethod.IsSynchronized = renamedMethod.IsSynchronized;
            interceptorMethod.IsUnmanaged = renamedMethod.IsUnmanaged;
            interceptorMethod.IsUnmanagedExport = renamedMethod.IsUnmanagedExport;
            interceptorMethod.IsVirtual = renamedMethod.IsVirtual;
            interceptorMethod.NoInlining = renamedMethod.NoInlining;
            interceptorMethod.NoOptimization = renamedMethod.NoOptimization;
        }

    }
}