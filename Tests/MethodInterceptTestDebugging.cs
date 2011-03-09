using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryoAOP.TestAssembly;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    [TestFixture]
    public class MethodInterceptTestDebugging
    {
        [Test]
        public void Should_be_able_to_debug_intercepted_method()
        {
            var instance = new TestMethodInterceptorType();
            instance.HavingMethodWithNoArgsAndNoReturnType();
        }
    }
}
