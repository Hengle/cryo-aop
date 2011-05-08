using System.Collections.Generic;
using System.Linq;
using CryoAOP.Core;
using CryoAOP.Core.Attributes;
using NUnit.Framework;
using Rhino.Mocks;

namespace CryoAOP.Tests.Core.Attributes
{
    [TestFixture]
    public class AttributeSearchTests
    {
        private AttributeSearch attributeSearch;
        private IEnumerable<AttributeResult<InterceptAttribute>> interceptAttributes;

        [TestFixtureSetUp]
        public void SetUp()
        {
            const string assemblyName = "CryoAOP.TestAssembly.dll";
            var assemblyImage = System.Reflection.Assembly.LoadFrom(assemblyName);

            var assemblyLoader = MockRepository.GenerateStub<IAssemblyLoader>();
            
            assemblyLoader
                .Expect(al => al.GetShadowAssemblies())
                .Return(new[] {new ShadowAssemblyType(assemblyImage, assemblyName)});

            attributeSearch = new AttributeSearch(assemblyLoader);

            interceptAttributes = attributeSearch.FindAttributes<InterceptAttribute>();
            
        }

        [Test]
        public void Should_find_intercept_attribute_for_type()
        {
            var typeAttribute =
                interceptAttributes
                    .Where(a => a.IsForType())
                    .FirstOrDefault();

            Assert.That(typeAttribute, Is.Not.Null);
            Assert.That(typeAttribute.IsForMethod(), Is.False);
            Assert.That(typeAttribute.IsForProperty(), Is.False);
        }

        [Test]
        public void Should_find_intercept_attribute_for_method()
        {
            var methodAttribute =
                interceptAttributes
                    .Where(a => a.IsForMethod())
                    .FirstOrDefault();

            Assert.That(methodAttribute, Is.Not.Null);
            Assert.That(methodAttribute.IsForType(), Is.False);
            Assert.That(methodAttribute.IsForProperty(), Is.False);
        }

        [Test]
        public void Should_find_intercept_attribute_for_property()
        {
            var propertyAttribute =
                interceptAttributes
                    .Where(a => a.IsForProperty())
                    .FirstOrDefault();

            Assert.That(propertyAttribute, Is.Not.Null);
            Assert.That(propertyAttribute.IsForType(), Is.False);
            Assert.That(propertyAttribute.IsForMethod(), Is.False);
        }
    }
}