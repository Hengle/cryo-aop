using System;
using System.Linq;
using System.Threading;
using CryoAOP.Core.Cache;
using NUnit.Framework;

namespace CryoAOP.Tests.Core.Cache
{
    [TestFixture]
    public class MemoryCacheTests
    {
        private readonly IMemoryCache memoryCache = new MemoryCache();

        [Test]
        public void Should_add_and_get_value_correctly()
        {
            memoryCache.Add("1", 1);
            Assert.That(memoryCache.Get("1"), Is.EqualTo(1));
        }

        [Test]
        public void Should_remove_value_correctly()
        {
            memoryCache.Add("2", 2);
            memoryCache.Remove("2");
            Assert.That(memoryCache.Get("2"), Is.Null);
        }

        [Test]
        public void Should_expire_value_correctly()
        {
            memoryCache.Add("3", 3, TimeSpan.FromMilliseconds(100));
            Thread.Sleep(200);
            Assert.That(memoryCache.Get("3"), Is.Null);
        }

        [Test]
        public void Should_set_and_get_value_correctly()
        {
            memoryCache.Set("4", 4);
            Assert.That(memoryCache.Get("4"), Is.EqualTo(4));
        }

        [Test]
        public void Should_replace_value_correctly()
        {
            memoryCache.Set("5", 5);
            memoryCache.Replace("5", 6);
            Assert.That(memoryCache.Get("5"), Is.EqualTo(6));
        }

        [Test]
        public void Should_clear_cache_correctly()
        {
            memoryCache.Set("6", 6);
            memoryCache.ClearAll();
            Assert.That(memoryCache.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Should_enumerate_keys_correctly()
        {
            memoryCache.Set("7", 7);
            Assert.That(memoryCache, Contains.Item("7"));
        }
    }
}