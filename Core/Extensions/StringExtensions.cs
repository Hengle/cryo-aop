using System;

namespace CryoAOP.Core.Extensions
{
    public static class StringExtensions
    {
        public static string FormatWith(this string s, params object[] args)
        {
            if (args == null || args.Length == 0) throw new ArgumentNullException("args");
            return string.Format(s, args);
        }
    }
}