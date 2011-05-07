using System;
using CryoAOP.Core.Extensions;

namespace CryoAOP.Exec
{
    internal static class ErrorExtensions
    {
        public static bool DisableWarnings;

        public static void Error(this string message, params object[] args)
        {
            if (args == null || args.Length == 0)
                Console.WriteLine(message);
            else
                Console.WriteLine(message.FormatWith(args));
        }

        public static void Warn(this string message, params object[] args)
        {
            if (DisableWarnings) return;

            if (args == null || args.Length == 0)
                Console.WriteLine(message);
            else
                Console.WriteLine(message.FormatWith(args));
        }
    }
}