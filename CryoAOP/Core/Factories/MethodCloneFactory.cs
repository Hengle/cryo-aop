//CryoAOP. Aspect Oriented Framework for .NET.
//Copyright (C) 2011  Gavin van der Merwe (fir3pho3nixx@gmail.com)

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Linq;
using CryoAOP.Core.Extensions;
using Mono.Cecil;

namespace CryoAOP.Core.Factories
{
    internal class MethodCloneFactory
    {
        private readonly MethodContext context;

        public MethodCloneFactory(MethodContext context)
        {
            this.context = context;
        }

        public MethodDefinition Clone(MethodReference sourceMethod)
        {
            return CloneIntoType(sourceMethod, sourceMethod.Resolve().DeclaringType);
        }

        public MethodDefinition CloneIntoType(MethodReference sourceMethod, TypeDefinition type)
        {
            var s = sourceMethod.Resolve();
            var c = new MethodDefinition(s.Name, s.Attributes, s.ReturnType);
            type.Methods.Add(c);

            CloneMethodProperties(c, s);
            CloneMethodAttributes(c, s);
            CloneMethodParameters(s, c);
            CloneGenericParameters(s, c);

            return c;
        }

        private void CloneMethodParameters(MethodReference sourceMethod, MethodReference clonedMethod)
        {
            if (sourceMethod.HasParameters)
            {
                var importer = new ImporterFactory(context);
                foreach (var parameter in sourceMethod.Parameters.ToList())
                {
                    var importedParameterType = importer.Import(parameter.ParameterType);
                    var parameterDef = 
                        new ParameterDefinition(
                            parameter.Name, 
                            parameter.Attributes,
                            importedParameterType);

                    clonedMethod.Parameters.Add(parameterDef);
                }
            }
        }

        private static void CloneMethodAttributes(MethodReference sourceMethod, MethodReference clonedMethod)
        {
            var s = sourceMethod.Resolve();
            var c = clonedMethod.Resolve();
            c.CallingConvention = s.CallingConvention;
            c.SemanticsAttributes = s.SemanticsAttributes;
            s.CustomAttributes.ToList().ForEach(a => c.CustomAttributes.Add(a));
            s.SecurityDeclarations.ToList().ForEach(sd => c.SecurityDeclarations.Add(sd));
        }

        private static void CloneMethodProperties(MethodReference sourceMethod, MethodReference clonedMethod)
        {
            var s = sourceMethod.Resolve();
            var c = clonedMethod.Resolve();
            c.IsAbstract = s.IsAbstract;
            c.IsAddOn = s.IsAddOn;
            c.IsAssembly = s.IsAssembly;
            c.IsCheckAccessOnOverride = s.IsCheckAccessOnOverride;
            c.IsCompilerControlled = s.IsCompilerControlled;
            c.IsFamily = s.IsFamily;
            c.IsFamilyAndAssembly = s.IsFamilyAndAssembly;
            c.IsFamilyOrAssembly = s.IsFamilyOrAssembly;
            c.IsFinal = s.IsFinal;
            c.IsFire = s.IsFire;
            c.IsForwardRef = s.IsForwardRef;
            c.IsGetter = s.IsGetter;
            c.IsHideBySig = s.IsHideBySig;
            c.IsIL = s.IsIL;
            c.IsInternalCall = s.IsInternalCall;
            c.IsManaged = s.IsManaged;
            c.IsNative = s.IsNative;
            c.IsNewSlot = s.IsNewSlot;
            c.IsPInvokeImpl = s.IsPInvokeImpl;
            c.IsPreserveSig = s.IsPreserveSig;
            c.IsPrivate = s.IsPrivate;
            c.IsPublic = s.IsPublic;
            c.IsRemoveOn = s.IsRemoveOn;
            c.IsReuseSlot = s.IsReuseSlot;
            c.IsRuntime = s.IsRuntime;
            c.IsRuntimeSpecialName = s.IsRuntimeSpecialName;
            c.IsSetter = s.IsSetter;
            c.IsSpecialName = s.IsSpecialName;
            c.IsStatic = s.IsStatic;
            c.IsSynchronized = s.IsSynchronized;
            c.IsUnmanaged = s.IsUnmanaged;
            c.IsUnmanagedExport = s.IsUnmanagedExport;
            c.IsVirtual = s.IsVirtual;
            c.NoInlining = s.NoInlining;
            c.NoOptimization = s.NoOptimization;
        }

        private void CloneGenericParameters(MethodReference sourceMethod, MethodReference clonedMethod)
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