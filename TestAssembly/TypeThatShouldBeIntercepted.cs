using System.Windows.Forms;
using CryoAOP.Core;
using CryoAOP.Core.Extensions;

namespace CryoAOP.TestAssembly
{
    public class TypeThatShouldBeIntercepted
    {
        public void HavingMethodWithNoArgsAndNoReturnType()
        {
            var methodInvocation = new MethodInvocation(typeof (TypeThatShouldBeIntercepted), null, null);
            methodInvocation.CancelInvocation();

            if (methodInvocation.CanInvoke)
                HavingMethodWithNoArgsAndInt32ReturnType();
        }

        public int HavingMethodWithNoArgsAndInt32ReturnType()
        {
            return 1;
        }

        public void HavingMethodWithArgsAndNoReturnType(int arg1, string arg2, double arg3)
        {
            MessageBox.Show("-> TypeThatShouldBeIntercepted::HavingMethodWithArgsAndNoReturnType(int={0}, string={1}, double={2}) returns Void".FormatWith(arg1, arg2, arg3));
        }

        public string HavingMethodWithArgsAndStringReturnType(int arg1, string arg2, double arg3)
        {
            var result = "{0}, {1}, {2}".FormatWith(arg1, arg2, arg3);
            MessageBox.Show("-> TypeThatShouldBeIntercepted::HavingMethodWithArgsAndStringReturnType(int={0}, string={1}, double={2}) returns string={3}".FormatWith(arg1, arg2, arg3, result));
            return result;
        }

        public void HavingMethodWithClassArgsAndNoReturnType(MethodParameterClass arg1)
        {
            GlobalInterceptor.HandleInvocation(new MethodInvocation(typeof (TypeThatShouldBeIntercepted), null, null));
        }

        public MethodParameterClass HavingMethodWithClassArgsAndClassReturnType(MethodParameterClass arg1)
        {
            return new MethodParameterClass();
        }
    }

    public class MethodParameterClass
    {
        public int Arg1 = -1;
    }
}