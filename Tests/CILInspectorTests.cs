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
        public void Method_Invocation_Builder()
        {
            new MethodInterceptorCILTemplate().InstanceMethodToBeIntercepted(1,"2", 3m);
        }

        [Test]
        public void test()
        {
            var assemblyInspector = new AssemblyInspector("CryoAOP.TestAssembly");
            var typeInspector = assemblyInspector.FindType(typeof (MethodInterceptorCILTemplate));
            var methodInspector = typeInspector.FindMethod("InstanceMethodToBeIntercepted");

            // Intercept and Clone
            methodInspector.InterceptAndClone("CryoAOP");


            // Write out intercepted method
            assemblyInspector.Definition.Write("CryoAOP.TestAssembly_Cloned_Method.dll");

            // Use reflection to invoke original method via clone 
            var assembly = Assembly.LoadFrom("CryoAOP.TestAssembly_Cloned_Method.dll");
            var fookedType = assembly.GetTypes().Where(t => t.FullName == typeof (MethodInterceptorCILTemplate).FullName).First();
            var methodInfo = fookedType.GetMethod(methodInspector.Definition.Name);
            methodInfo.Invoke(assembly.CreateInstance(fookedType.FullName), new object[] {1, "Foo", 3m});
        }
    }
}
