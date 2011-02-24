using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CryoAOP.Core.Extensions;
using CryoAOP.TestAssembly;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    public class ClassWithClonedMethod
    {
        public void GenericMethod<T, J>(T t, J j)
        {
            //return default(T);
        }
    }

    public class PanicStationFixture
    {
        [Test]
        public void should_emit_call_to_generic_properly()
        {
            var assembly = AssemblyDefinition.ReadAssembly("CryoAOP.Tests.dll");
            var type = assembly.MainModule.Types.Where(t => t.Name == "ClassWithClonedMethod").First();
            var renamedMethod = type.Methods.Where(m => m.Name == "GenericMethod").First();

            // Create new method 
            var methodThatShouldReplaceOriginal = new MethodDefinition(renamedMethod.Name, renamedMethod.Attributes, renamedMethod.ReturnType);
            renamedMethod.DeclaringType.Methods.Add(methodThatShouldReplaceOriginal);

            // Rename existing method 
            renamedMethod.Name = string.Format("{0}_Clone", renamedMethod.Name);

            // Copy vars and stuff
            methodThatShouldReplaceOriginal.CallingConvention = renamedMethod.CallingConvention;
            methodThatShouldReplaceOriginal.SemanticsAttributes = renamedMethod.SemanticsAttributes;
            renamedMethod.CustomAttributes.ToList().ForEach(a => methodThatShouldReplaceOriginal.CustomAttributes.Add(a));
            renamedMethod.SecurityDeclarations.ToList().ForEach(s => methodThatShouldReplaceOriginal.SecurityDeclarations.Add(s));

            // Copy parameters across
            if (renamedMethod.HasParameters)
                foreach (var parameter in renamedMethod.Parameters.ToList())
                    methodThatShouldReplaceOriginal.Parameters.Add(parameter);


            // Copy generic parameters across
            if (renamedMethod.HasGenericParameters)
            {
                foreach (var genericParameter in renamedMethod.GenericParameters.ToList())
                {
                    if (genericParameter != null)
                    {
                        var newGenericParameter = new GenericParameter(genericParameter.Name, methodThatShouldReplaceOriginal);
                        methodThatShouldReplaceOriginal.GenericParameters.Add(newGenericParameter);
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

            // Get IL processor and emit call to renamed clone
            var il = methodThatShouldReplaceOriginal.Body.GetILProcessor();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Call, renamedMethod.MakeGeneric(methodThatShouldReplaceOriginal.GenericParameters.ToArray()));
            il.Emit(OpCodes.Ret);

            assembly.Write("Foo.dll");

            var fooAssembly = Assembly.LoadFrom("Foo.dll");
            var methodInfo = GetGenericMethodInfo(fooAssembly, "ClassWithClonedMethod", "GenericMethod", typeof(int), typeof(string));
            AutoInstanceInvoke(methodInfo, 1, "Foo");
        }

        public static Type FindType(Assembly assembly, string typeName)
        {
            var type = assembly.GetTypes().Where(t => t.FullName.ToLower().EndsWith(typeName.ToLower())).FirstOrDefault();
            if (type == null)
                throw new Exception("Could not find type '{0}' in '{1}'".FormatWith(typeName, assembly.FullName));
            return type;
        }

        public static object AutoInstanceInvoke(MethodInfo method, params object[] args)
        {
            var type = method.DeclaringType;
            var instance = type.Assembly.CreateInstance(type.FullName);
            var returnValue = method.Invoke(instance, args);
            return returnValue;
        }

        private static MethodInfo GetGenericMethodInfo(Assembly assembly, string typeName, string methodName, params Type[] genericTypeParameter)
        {
            var interceptedType = assembly.FindType(typeName);
            MethodInfo genericMethodInfo = interceptedType.GetMethod(methodName);
            return genericMethodInfo.MakeGenericMethod(genericTypeParameter);
        }
    }
}
