using System.Collections.Generic;
using System.Diagnostics;
using CryoAOP.Core;
using CryoAOP.Core.Extensions;

namespace CryoAOP.TestAssembly
{
    public class MethodInterceptorCILTemplate
    {
        public void InstanceMethodToBeIntercepted(int i, string j, decimal k)
        {
            var type = typeof(MethodInterceptorCILTemplate);
            var method = type.GetMethod("InstanceMethodToBeIntercepted");
            var parameters = new List<object>();
            parameters.Add(i);
            parameters.Add(j);
            parameters.Add(k);
            var invocation = new MethodInvocation(type, method, parameters);
            GlobalInterceptor.HandleInvocation(invocation);


            Debug.WriteLine("Method has fired! i={0}, j={1}, k={2}".FormatWith(i, j, k));
        }
    }
}