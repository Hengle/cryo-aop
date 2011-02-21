using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CryoAOP.Core;
using CryoAOP.Core.Extensions;
using CryoAOP.TestAssembly;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.PE;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    [TestFixture]
    public class MethodInspectorTests
    {
        private static Assembly assembly;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            GlobalInterceptor.Clear();
        }

        #endregion

        private const string interceptedOutputAssembly = "CryoAOP.TestAssembly_Intercepted.dll";

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            //Interception.RegisterType("CryoAOP.TestAssembly", "CryoAOP.TestAssembly.TypeThatShouldBeIntercepted", "GenericMethod", interceptedOutputAssembly);
            Interception.RegisterType("CryoAOP.TestAssembly", "CryoAOP.TestAssembly.TypeThatShouldBeIntercepted", interceptedOutputAssembly);
            assembly = Assembly.LoadFrom(interceptedOutputAssembly);
        }

        private static MethodInfo GetNonGenericMethodInfo(string nonGenericMethodName)
        {
            var interceptedType = assembly.FindType(typeof(TypeThatShouldBeIntercepted).FullName);
            return interceptedType.GetMethod(nonGenericMethodName);
        }

        private static MethodInfo GetGenericMethodInfo(string nonGenericMethodName, Type genericTypeParameter)
        {
            var interceptedType = assembly.FindType(typeof(TypeThatShouldBeIntercepted).FullName);
            MethodInfo genericMethodInfo = interceptedType.GetMethod(nonGenericMethodName);
            return genericMethodInfo.MakeGenericMethod(genericTypeParameter);
        }

        [Test]
        public void Should_intercept_method_and_call()
        {
            var interceptCount = 0;
            var methodInfo = GetNonGenericMethodInfo("HavingMethodWithArgsAndNoReturnType");

            GlobalInterceptor.MethodIntercepter += (invocation) => { interceptCount++; };

            var result = methodInfo.Invoke(1, "2", 3);
            Assert.That(interceptCount, Is.EqualTo(2));
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Should_intercept_method_and_cancel_invocation_returning_fake_result()
        {
            var methodInfo = GetNonGenericMethodInfo("HavingMethodWithArgsAndStringReturnType");

            GlobalInterceptor.MethodIntercepter += (invocation) =>
            {
                if (invocation.InvocationType == MethodInvocationType.BeforeInvocation)
                {
                    invocation.CancelInvocation();
                    invocation.Result = "Fake Result";
                }
            };

            var result = methodInfo.Invoke(1, "2", 3);
            Assert.That(result, Is.EqualTo("Fake Result"));
        }

        [Test]
        public void Should_intercept_method_with_class_args_and_call()
        {
            var interceptCount = 0;
            var methodInfo = GetNonGenericMethodInfo("HavingMethodWithClassArgsAndClassReturnType");
            var args = assembly.CreateInstance("CryoAOP.TestAssembly.MethodParameterClass");

            GlobalInterceptor.MethodIntercepter += (invocation) => { interceptCount++; };

            var result = methodInfo.Invoke(args);
            Assert.That(interceptCount, Is.EqualTo(2));
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void Should_intercept_method_and_call_using_reflection_changing_return_value()
        {
            var methodInfo = GetNonGenericMethodInfo("HavingMethodWithArgsAndStringReturnType");

            GlobalInterceptor.MethodIntercepter += (invocation) =>
                                                       {
                                                           if (invocation.InvocationType == MethodInvocationType.AfterInvocation)
                                                               invocation.Result = "Intercepted Result";
                                                       };

            var result = methodInfo.Invoke(1, "2", 3);
            Assert.That(result, Is.EqualTo("Intercepted Result"));
        }

        [Test]
        public void Should_intercept_method_and_call_with_result()
        {
            var interceptCount = 0;
            var methodInfo = GetNonGenericMethodInfo("HavingMethodWithArgsAndStringReturnType");

            GlobalInterceptor.MethodIntercepter += (invocation) => { interceptCount++; };

            var result = methodInfo.Invoke(1, "2", 3);
            Assert.That(interceptCount, Is.EqualTo(2));
            Assert.That(result, Is.EqualTo("1, 2, 3"));
        }

        [Test]
        public void Should_call_to_generic_method()
        {
            var interceptorCount = 0;
            var methodInfo = GetGenericMethodInfo("GenericMethod", typeof(MethodParameterClass));

            GlobalInterceptor.MethodIntercepter += (invocation) => { interceptorCount++; };

            methodInfo.Invoke();
            Assert.That(interceptorCount, Is.EqualTo(2));
        }

        /*
        [Test]
        public void should_emit_call_to_generic_properly()
        {
            var assembly = AssemblyDefinition.ReadAssembly("CryoAOP.TestAssembly.dll");
            var type = assembly.MainModule.Types.Where(t => t.Name == "TypeThatShouldBeIntercepted").First();
            var renamedMethod = type.Methods.Where(m => m.Name == "GenericMethod").First();

            // Create new method 
            MethodDefinition methodThatShouldReplaceOriginal = new MethodDefinition(renamedMethod.Name, renamedMethod.Attributes, renamedMethod.ReturnType);
            renamedMethod.DeclaringType.Methods.Add(methodThatShouldReplaceOriginal);

            // Rename existing method 
            renamedMethod.Name = string.Format("{0}_Clone", renamedMethod.Name);

            // Copy vars and stuff
            methodThatShouldReplaceOriginal.CallingConvention = renamedMethod.CallingConvention;
            methodThatShouldReplaceOriginal.SemanticsAttributes = renamedMethod.SemanticsAttributes;
            renamedMethod.CustomAttributes.ToList().ForEach(a => methodThatShouldReplaceOriginal.CustomAttributes.Add(a));
            renamedMethod.SecurityDeclarations.ToList().ForEach(s => methodThatShouldReplaceOriginal.SecurityDeclarations.Add(s));

            methodThatShouldReplaceOriginal.IsAbstract = renamedMethod.IsAbstract;
            methodThatShouldReplaceOriginal.IsAddOn = renamedMethod.IsAddOn;
            methodThatShouldReplaceOriginal.IsAssembly = renamedMethod.IsAssembly;
            methodThatShouldReplaceOriginal.IsCheckAccessOnOverride = renamedMethod.IsCheckAccessOnOverride;
            methodThatShouldReplaceOriginal.IsCompilerControlled = renamedMethod.IsCompilerControlled;
            methodThatShouldReplaceOriginal.IsFamily = renamedMethod.IsFamily;
            methodThatShouldReplaceOriginal.IsFamilyAndAssembly = renamedMethod.IsFamilyAndAssembly;
            methodThatShouldReplaceOriginal.IsFamilyOrAssembly = renamedMethod.IsFamilyOrAssembly;
            methodThatShouldReplaceOriginal.IsFinal = renamedMethod.IsFinal;
            methodThatShouldReplaceOriginal.IsFire = renamedMethod.IsFire;
            methodThatShouldReplaceOriginal.IsForwardRef = renamedMethod.IsForwardRef;
            methodThatShouldReplaceOriginal.IsGetter = renamedMethod.IsGetter;
            methodThatShouldReplaceOriginal.IsHideBySig = renamedMethod.IsHideBySig;
            methodThatShouldReplaceOriginal.IsIL = renamedMethod.IsIL;
            methodThatShouldReplaceOriginal.IsInternalCall = renamedMethod.IsInternalCall;
            methodThatShouldReplaceOriginal.IsManaged = renamedMethod.IsManaged;
            methodThatShouldReplaceOriginal.IsNative = renamedMethod.IsNative;
            methodThatShouldReplaceOriginal.IsNewSlot = renamedMethod.IsNewSlot;
            methodThatShouldReplaceOriginal.IsPInvokeImpl = renamedMethod.IsPInvokeImpl;
            methodThatShouldReplaceOriginal.IsPreserveSig = renamedMethod.IsPreserveSig;
            methodThatShouldReplaceOriginal.IsPrivate = renamedMethod.IsPrivate;
            methodThatShouldReplaceOriginal.IsPublic = renamedMethod.IsPublic;
            methodThatShouldReplaceOriginal.IsRemoveOn = renamedMethod.IsRemoveOn;
            methodThatShouldReplaceOriginal.IsReuseSlot = renamedMethod.IsReuseSlot;
            methodThatShouldReplaceOriginal.IsRuntime = renamedMethod.IsRuntime;
            methodThatShouldReplaceOriginal.IsRuntimeSpecialName = renamedMethod.IsRuntimeSpecialName;
            methodThatShouldReplaceOriginal.IsSetter = renamedMethod.IsSetter;
            methodThatShouldReplaceOriginal.IsSpecialName = renamedMethod.IsSpecialName;
            methodThatShouldReplaceOriginal.IsStatic = renamedMethod.IsStatic;
            methodThatShouldReplaceOriginal.IsSynchronized = renamedMethod.IsSynchronized;
            methodThatShouldReplaceOriginal.IsUnmanaged = renamedMethod.IsUnmanaged;
            methodThatShouldReplaceOriginal.IsUnmanagedExport = renamedMethod.IsUnmanagedExport;
            methodThatShouldReplaceOriginal.IsVirtual = renamedMethod.IsVirtual;
            methodThatShouldReplaceOriginal.NoInlining = renamedMethod.NoInlining;
            methodThatShouldReplaceOriginal.NoOptimization = renamedMethod.NoOptimization;

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
            il.Emit(OpCodes.Call, renamedMethod.MakeGeneric(methodThatShouldReplaceOriginal.GenericParameters.ToArray()));
            il.Emit(OpCodes.Ret);

            assembly.Write("Foo.dll");
        }
        */

    }

    /*
    public static class Foo
    {
        public static MethodReference MakeGeneric(this MethodReference method, params TypeReference[] args)
        {
            if (args.Length == 0)
                return method;

            if (method.GenericParameters.Count != args.Length)
                throw new ArgumentException("Invalid number of generic type arguments supplied");

            var genericTypeRef = new GenericInstanceMethod(method);
            foreach (var arg in args)
                genericTypeRef.GenericArguments.Add(arg);

            return genericTypeRef;
        }
    }
     * */
}