using System;
using System.Collections.Generic;

namespace CryoAOP.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> e, Action<T> action)
        {
            foreach (var t in e)
                action(t);
        }
    }
}