using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            cilType.InstanceMethodToBeIntercepted();
        }
    }
}
