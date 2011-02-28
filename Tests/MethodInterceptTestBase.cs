using System.Reflection;
using CryoAOP.Core;
using NUnit.Framework;

namespace CryoAOP.Tests
{
    public class MethodInterceptTestBase
    {
        protected static Assembly InterceptedAssembly;

        protected virtual string InputAssembly
        {
            get { return "CryoAOP.TestAssembly.dll"; }
        }

        protected virtual string OutputAssembly
        {
            get { return "CryoAOP.TestAssembly_Intercepted.dll"; }
        }

        protected virtual string InterceptType
        {
            get { return "CryoAOP.TestAssembly.TestMethodInterceptorType"; }
        }

        protected virtual string DebuggingInterceptorMethod
        {
            get { return null; }
        }

        protected virtual MethodInterceptionScope MethodInterceptionScope
        {
            get { return MethodInterceptionScope.Shallow; }
        }

        protected virtual bool IsDebugging
        {
            get { return DebuggingInterceptorMethod != null || MethodInterceptionScope != MethodInterceptionScope.Shallow; }
        }

        [TestFixtureSetUp]
        public virtual void FixtureSetUp()
        {
            Intercept.LoadAssembly(InputAssembly);
            
            if (DebuggingInterceptorMethod == null)
                Intercept.InterceptType(InterceptType, MethodInterceptionScope);
            else
                Intercept.InterceptMethod(InterceptType, DebuggingInterceptorMethod, MethodInterceptionScope);
            
            Intercept.SaveAssembly(OutputAssembly);
            InterceptedAssembly = Assembly.LoadFrom(OutputAssembly);
        }

        [SetUp]
        public void SetUp()
        {
            Intercept.Clear();
        }
    }
}