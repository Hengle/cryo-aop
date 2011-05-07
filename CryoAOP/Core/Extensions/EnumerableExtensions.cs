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

#if !NET_3_5 && !NET_4_0

namespace System
{
    internal delegate TResult Func<TResult>();
    internal delegate TResult Func<T, TResult>(T arg1);
    internal delegate TResult Func<T1, T2, TResult>(T1 arg1, T2 arg2);
    internal delegate TResult Func<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3);
    internal delegate TResult Func<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    
    internal delegate void Action ();
    internal delegate void Action<T1, T2> (T1 arg1, T2 arg2);
    internal delegate void Action<T1, T2, T3> (T1 arg1, T2 arg2, T3 arg3);
    internal delegate void Action<T1, T2, T3, T4> (T1 arg1, T2 arg2, T3 arg3, T4 arg4);
}

namespace System.Linq
{
    internal static class Enumerable
    {
        internal static bool Any<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            foreach (var item in items)
                if (predicate(item))
                    return true;
            return false;
        }

        internal static IEnumerable<T> Where<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            var found = new List<T>();
            foreach (var item in items)
                if (predicate(item))
                    found.Add(item);
            return found;
        }

        internal static int Count<T>(this IEnumerable<T> items)
        {
            int counter = 0;
            foreach (var item in items)
                counter++;
            return counter;
        }

        internal static T ElementAt<T>(this IEnumerable<T> items, int index)
        {
            int counter = 0;
            foreach (var item in items)
            {
                if (counter == index)
                    return item;
                counter++;
            }
            return default(T);
        }

        internal static T First<T>(this IEnumerable<T> items)
        {
            if (items.Count() > 0)
                return items.ElementAt(0);
            throw new Exception("Value could not be found!");
        }

        internal static T Last<T>(this IEnumerable<T> items)
        {
            if (items.Count() > 0)
                return items.ElementAt(items.Count()-1);
            throw new Exception("Value could not be found!");
        }

        internal static T First<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            foreach (var item in items)
                if (predicate(item))
                    return item;
            throw new Exception("Value could not be found!");
        }

        internal static T FirstOrDefault<T>(this IEnumerable<T> items)
        {
            if (items.Count() > 0)
                return items.ElementAt(0);
            return default(T);
        }

        internal static T FirstOrDefault<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            foreach (var item in items)
                if (predicate(item))
                    return item;
            return default(T);
        }

        internal static bool All<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            foreach (TSource local in source)
            {
                if (!predicate(local))
                {
                    return false;
                }
            }
            return true;
        }

        internal static IEnumerable<IEnumerable<TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var keys = new List<TKey>();
            foreach (var item in source)
            {
                var key = keySelector(item);
                if (!keys.Contains(key))
                    keys.Add(key);
            }

            var results = new List<IEnumerable<TSource>>();
            foreach (var key in keys)
            {
                var grouping = new List<TSource>();
                foreach (var item in source)
                {
                    if (keySelector(item).Equals(key))
                        grouping.Add(item);
                }
                results.Add(grouping);
            }

            return results;
        }

        public static IList<T> ToList<T>(this IEnumerable<T> items)
        {
            var list = new List<T>();
            foreach (var item in items)
            {
                list.Add(item);
            }
            return list;
        }

        public static IEnumerable<T> Cast<T>(this IEnumerable items)
        {
            var list = new List<T>();
            foreach (var item in items)
                list.Add((T)item);
            return list;
        }

        public static T[] ToArray<T>(this IEnumerable<T> items)
        {
            var index = 0;
            var list = new T[items.Count()];
            foreach (var item in items)
            {
                list[index++] = item;
            }
            return list;
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> items, IEnumerable<T> exclusionItems)
        {
            var list = new List<T>();
            foreach (var item in items)
            {
                var found = false;
                foreach (var search in exclusionItems)
                {
                    if (item.Equals(search))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    list.Add(item);
            }
            return list;
        }

        public static IEnumerable<S> Select<T, S>(this IEnumerable<T> items, Func<T, S> selector)
        {
            var list = new List<S>();
            foreach (var item in items)
            {
                var selected = selector(item);
                list.Add(selected);
            }
            return list;
        }

        public static bool Contains<T>(this IEnumerable<T> items, T element)
        {
            foreach (var item in items)
            {
                if (item.Equals(element))
                    return true;
            }
            return false;
        }

        public static IEnumerable<T> Take<T>(this IEnumerable<T> items, int number)
        {
            int counter = 0;
            var list = new List<T>();
            foreach (var item in items)
            {
                if (counter < number)
                    list.Add(item);

                if (counter == number)
                    break;

                counter = counter + 1;
            }
            return list;
        }
    }
}


#endif