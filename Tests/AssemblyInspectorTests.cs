using System;
using CryoAOP.Core;
using CryoAOP.Core.Exceptions;
using CryoAOP.Core.Factories;
using NUnit.Framework;
using CryoAOP.TestAssembly;

namespace CryoAOP.Tests
{
    [TestFixture]
    public class AssemblyInspectorTests
    {
        private class TypeThatCannotBeFound { }

        protected string TestAssembly;
        protected AssemblyInspector AssemblyInspector;
        protected AssemblyImporterFactory ImporterFactory;

        [TestFixtureSetUp]
        public virtual void Setup_Fixture()
        {
            TestAssembly = "CryoAOP.TestAssembly.dll";
            AssemblyInspector = new AssemblyInspector(TestAssembly);
            ImporterFactory = new AssemblyImporterFactory(AssemblyInspector.Definition);
        }

        [Test]
        public void Should_find_type_that_should_be_intercepted_in_test_assembly()
        {
            var typeToFind = typeof(TypeThatShouldBeIntercepted);
            var typeDefinition = AssemblyInspector.FindType(typeToFind);

            Assert.That(typeDefinition, Is.Not.Null);
            Assert.That(typeDefinition.Definition.FullName, Is.EqualTo(typeToFind.FullName));
        }

        [Test]
        public void Should_find_type_reference()
        {
            var inspector = new AssemblyInspector(typeof (Type).Assembly);
            var typeReference = ImporterFactory.Import(typeof (Type));

            Assert.That(typeReference, Is.Not.Null);
        }

        [Test]
        [ExpectedException(typeof(TypeNotFoundException))]
        public void Should_throw_if_type_not_found()
        {
            var typeThatCannotBeFound = typeof(TypeThatCannotBeFound);
            AssemblyInspector.FindType(typeThatCannotBeFound);
        }

        [Test]
        [ExpectedException(typeof(AssemblyNotFoundException))]
        public void Should_throw_when_assembly_not_found()
        {
            new AssemblyInspector("Some assembly that does not exist");
        }
    }
}
