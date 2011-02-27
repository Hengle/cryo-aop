using System;
using CryoAOP.Core.Extensions;

namespace CryoAOP
{
    public static class ErrorExtensions
    {
        public static void Error(this string message, params object[] args)
        {
            if (args == null || args.Length == 0)
                Console.WriteLine("Error:{0}".FormatWith(message));
            else
                Console.WriteLine("Error:{0}".FormatWith(message, args));
        }

        public static void Error(string message, int lineNumber, params object[] args)
        {
            if (args == null || args.Length == 0)
                Console.WriteLine("Line:{0} - {1}".FormatWith(lineNumber, message));
            else
                Console.WriteLine("Line:{0} - {1}".FormatWith(lineNumber, message.FormatWith(args)));
        }
    }
}