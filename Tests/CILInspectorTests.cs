using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryoAOP.Core;
using CryoAOP.TestAssembly;
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
            var methodInspector = 
                new AssemblyInspector("CryoAOP.TestAssembly")
                    .FindType(typeof (MethodInterceptorCILTemplate))
                    .FindMethod("InstanceMethodToBeIntercepted");

            var methodInstructions = 
                methodInspector
                    .Definition
                    .Body
                    .Instructions;
        }
    }
}
