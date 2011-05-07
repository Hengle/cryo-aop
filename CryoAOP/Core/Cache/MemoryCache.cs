using System;
using System.Collections;
using System.Collections.Generic;

namespace CryoAOP.Core.Cache
{
    public interface IMemoryCache : IEnumerable<string>
    {
        void Dispose();
        bool Remove(string key);
        void RemoveAll(IEnumerable<string> keys);
        object Get(string key);
        object Get(string key, out long lastModifiedTicks);
        T Get<T>(string key);
        bool Add<T>(string key, T value);
        bool Set<T>(string key, T value);
        bool Replace<T>(string key, T value);
        bool Add<T>(string key, T value, DateTime expiresAt);
        bool Set<T>(string key, T value, DateTime expiresAt);
        bool Replace<T>(string key, T value, DateTime expiresAt);
        bool Add<T>(string key, T value, TimeSpan expiresIn);
        bool Set<T>(string key, T value, TimeSpan expiresIn);
        bool Replace<T>(string key, T value, TimeSpan expiresIn);
        void ClearAll();
        IDictionary<string, T> GetAll<T>(IEnumerable<string> keys);
        void SetAll<T>(IDictionary<string, T> values);
    }

    public class MemoryCache : IMemoryCache
    {
        private Dictionary<string, CacheEntry> memory;

        private class CacheEntry
        {
            private object cacheValue;

            public CacheEntry(object value, DateTime expiresAt)
            {
                Value = value;
                ExpiresAt = expiresAt;
                LastModifiedTicks = DateTime.Now.Ticks;
            }

            internal DateTime ExpiresAt { get; set; }

            internal object Value
            {
                get { return cacheValue; }
                set
                {
                    cacheValue = value;
                    LastModifiedTicks = DateTime.Now.Ticks;
                }
            }

            internal long LastModifiedTicks { get; private set; }
        }

        public MemoryCache()
        {
            this.memory = new Dictionary<string, CacheEntry>();
        }

        private bool CacheAdd(string key, object value)
        {
            return CacheAdd(key, value, DateTime.MaxValue);
        }

        private bool CacheAdd(string key, object value, DateTime expiresAt)
        {
            CacheEntry entry;
            if (this.memory.TryGetValue(key, out entry)) return false;

            entry = new CacheEntry(value, expiresAt);
            this.memory.Add(key, entry);

            return true;
        }

        private bool CacheSet(string key, object value)
        {
            return CacheSet(key, value, DateTime.MaxValue);
        }

        private bool CacheSet(string key, object value, DateTime expiresAt, long? checkLastModified = null)
        {
            CacheEntry entry;
            if (!this.memory.TryGetValue(key, out entry))
            {
                entry = new CacheEntry(value, expiresAt);
                this.memory.Add(key, entry);
                return true;
            }

            if (checkLastModified.HasValue
                && entry.LastModifiedTicks != checkLastModified.Value) return false;

            entry.Value = value;
            entry.ExpiresAt = expiresAt;

            return true;
        }

        private bool CacheReplace(string key, object value)
        {
            return CacheReplace(key, value, DateTime.MaxValue);
        }

        private bool CacheReplace(string key, object value, DateTime expiresAt)
        {
            return !CacheSet(key, value, expiresAt);
        }

        public void Dispose()
        {
            this.memory = null;
        }

        public bool Remove(string key)
        {
            if (this.memory.ContainsKey(key))
            {
                this.memory.Remove(key);
                return true;
            }
            return false;
        }

        public void RemoveAll(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                this.Remove(key);
            }
        }

        public object Get(string key)
        {
            long lastModifiedTicks;
            return Get(key, out lastModifiedTicks);
        }

        public object Get(string key, out long lastModifiedTicks)
        {
            lastModifiedTicks = 0;
            if (this.memory.ContainsKey(key))
            {
                var cacheEntry = this.memory[key];
                if (cacheEntry.ExpiresAt < DateTime.Now)
                {
                    this.memory.Remove(key);
                    return null;
                }
                lastModifiedTicks = cacheEntry.LastModifiedTicks;
                return cacheEntry.Value;
            }
            return null;
        }

        public T Get<T>(string key)
        {
            var value = Get(key);
            if (value != null) return (T)value;
            return default(T);
        }

        public bool Add<T>(string key, T value)
        {
            return CacheAdd(key, value);
        }

        public bool Set<T>(string key, T value)
        {
            return CacheSet(key, value);
        }

        public bool Replace<T>(string key, T value)
        {
            return CacheReplace(key, value);
        }

        public bool Add<T>(string key, T value, DateTime expiresAt)
        {
            return CacheAdd(key, value, expiresAt);
        }

        public bool Set<T>(string key, T value, DateTime expiresAt)
        {
            return CacheSet(key, value, expiresAt);
        }

        public bool Replace<T>(string key, T value, DateTime expiresAt)
        {
            return CacheReplace(key, value, expiresAt);
        }

        public bool Add<T>(string key, T value, TimeSpan expiresIn)
        {
            return CacheAdd(key, value, DateTime.Now.Add(expiresIn));
        }

        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            return CacheSet(key, value, DateTime.Now.Add(expiresIn));
        }

        public bool Replace<T>(string key, T value, TimeSpan expiresIn)
        {
            return CacheReplace(key, value, DateTime.Now.Add(expiresIn));
        }

        public void ClearAll()
        {
            this.memory = new Dictionary<string, CacheEntry>();
        }

        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            var valueMap = new Dictionary<string, T>();
            foreach (var key in keys)
            {
                var value = Get<T>(key);
                valueMap[key] = value;
            }
            return valueMap;
        }

        public void SetAll<T>(IDictionary<string, T> values)
        {
            foreach (var entry in values)
            {
                Set(entry.Key, entry.Value);
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return memory.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}