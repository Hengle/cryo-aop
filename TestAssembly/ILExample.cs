using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryoAOP.Core;

namespace CryoAOP.TestAssembly
{
    public class ILExample
    {
        public void Method(int i)
        {
        }

        public void CallToMethod(int i)
        {
            var type = typeof (ILExample);
            var method = type.GetMethod("CallToMethod");
            var @params = new object[0];
            var invoke = new MethodInvocation(this, type, method, @params);
            Intercept.HandleInvocation(invoke);
            if (invoke.CanInvoke)
            {
                Method(i);
                invoke.ContinueInvocation();
                Intercept.HandleInvocation(invoke);
            }
        }
    }
}
