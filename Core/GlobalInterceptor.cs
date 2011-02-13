using System.Windows.Forms;
using CryoAOP.Core.Extensions;

namespace CryoAOP.Core
{
    public class GlobalInterceptor
    {
        public static void HandleInvocation(Invocation invocation)
        {
            var methodInvocation = (MethodInvocation) invocation;
            MessageBox.Show("-> GlobalInterceptor::HandleInvocation(Invocation={0})".FormatWith(methodInvocation));
            //methodInvocation.CancelInvocation();
        }
    }
}