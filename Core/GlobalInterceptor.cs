using System;
using System.IO;
using CryoAOP.Core.Extensions;

namespace CryoAOP.Core
{
    public class GlobalInterceptor
    {
        public static void HandleInvocation(Invocation invocation)
        {
            using (var r = new StreamWriter(@"c:\out.txt", true))
                r.WriteLine("{0} -> public static void HandleInvocation(Invocation invocation)"
                                .FormatWith(DateTime.Now));
        }
    }
}