using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CryoAOP.Core.Cache
{
    public interface IMemoryCacheGeneric : IEnumerable<string>
    {
        T Get<T>(string key);
        void Set<T>(string key, T value);
        bool ContainsKey<T>(string key);


    }

    public class MemoryCacheGeneric : IMemoryCacheGeneric
    {
        private readonly IDictionary<System.Type, IMemoryCache> typeCache = new Dictionary<System.Type, IMemoryCache>();
        public TimeSpan DefaultExpiry = TimeSpan.FromMinutes(30);

        #region IMemoryCacheGeneric Members

        public virtual T Get<T>(string key)
        {
            var cache = GetCache<T>();
            return cache.Get<T>(key);
        }

        public virtual void Set<T>(string key, T value)
        {
            var cache = GetCache<T>();
            cache.Set(key, value, DefaultExpiry);
        }

        public bool ContainsKey<T>(string key)
        {
            var cache = GetCache<T>();
            return cache.ContainsKey(key);
        }

        public IEnumerator<string> GetEnumerator()
        {
            var items = new List<string>();
            foreach (var cacheKey in typeCache.Keys)
            {
                var cache = typeCache[cacheKey];
                foreach (var key in cache)
                {
                    items.Add(key);
                }
            }
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private IMemoryCache GetCache<T>()
        {
            IMemoryCache cache;
            if (!typeCache.TryGetValue(typeof (T), out cache))
            {
                cache = new MemoryCache();
                typeCache.Add(typeof (T), cache);
            }
            return cache;
        }
    }
}