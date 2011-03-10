using System;
using System.Collections.Generic;
using System.Linq;

namespace CryoAOP.Core.Extensions
{
    public static class StringExtensions
    {
        public static string FormatWith(this string s, params object[] args)
        {
            if (args == null || args.Length == 0) throw new ArgumentNullException("args");
            return string.Format(s, args);
        }

        public static string JoinWith<T>(this IEnumerable<T> items, string delimeter)
        {
            return string.Join(delimeter, items.Cast<string>().ToArray());
        }
    }
}