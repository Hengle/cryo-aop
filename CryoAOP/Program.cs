using System;

namespace CryoAOP
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("CryoAOP v1.0");
                Console.WriteLine("Usage: CryoAOP /i input.cryoaop");
                Console.WriteLine("Where: /i input.cryoaop is of the following format.");
                Console.WriteLine("Assembly->TypeToIntercept->Method(<overload_specification>)");
                Console.WriteLine("Example:");
                Console.WriteLine("FooAssembly.dll -> Foo.TypeToIntercept -> MethodToIntercept();");
                Console.WriteLine("FooAssembly.dll -> Foo.TypeToIntercept -> MethodToIntercept(string,int);");
            }
        }
    }
}