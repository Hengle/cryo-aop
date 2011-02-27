using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CryoAOP.Core;

namespace CryoAOP.TestAssembly
{
    public class ILExample
    {
        public static void Method(int i)
        {
        }

        public static void CallToMethod(int i)
        {
            var type = typeof (ILExample);
            var method = type.GetMethod("CallToMethod");
            var @params = new object[0];
            var invoke = new MethodInvocation(type, method, @params);
            GlobalInterceptor.HandleInvocation(invoke);
            if (invoke.CanInvoke)
            {
                Method(i);
                invoke.ContinueInvocation();
                GlobalInterceptor.HandleInvocation(invoke);
            }
        }
    }
}
