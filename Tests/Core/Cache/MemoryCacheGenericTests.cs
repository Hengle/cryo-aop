#if NET_3_5 || NET_4_0

using System.Linq;
using CryoAOP.Core.Cache;
using NUnit.Framework;

namespace CryoAOP.Tests.Core.Cache
{
    [TestFixture]
    public class MemoryCacheGenericTests
    {
        private readonly IMemoryCacheGeneric genericCache = new MemoryCacheGeneric();

        [Test]
        public void Should_enumerate_keys_correctly_for_different_types()
        {
            genericCache.Set("2", "Two");
            genericCache.Set("3", 3m);
            genericCache.Set("4", 4f);

            Assert.That(genericCache.ToList(), Contains.Item("2"));
            Assert.That(genericCache.ToList(), Contains.Item("3"));
            Assert.That(genericCache.ToList(), Contains.Item("4"));
        }

        [Test]
        public void Should_set_and_get_value_for_generic_type_correctly()
        {
            genericCache.Set("1", 1);
            Assert.That(genericCache.Get<int>("1"), Is.Not.Null);
        }
    }
}

#endif