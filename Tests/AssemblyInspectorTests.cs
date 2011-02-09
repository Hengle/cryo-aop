using CryoAOP.Core;
using NUnit.Framework;
using CryoAOP.TestAssembly;

namespace CryoAOP.Tests
{
    public class TypeThatCannotBeFound { }

    [TestFixture]
    public class AssemblyInspectorTests
    {
        private const string TestAssembly = "CryoAOP.TestAssembly.dll";
        private AssemblyInspector TestInspector = new AssemblyInspector(TestAssembly);

        [Test]
        public void Should_find_type_that_should_be_intercepted_in_test_assembly()
        {
            var typeToFind = typeof (TypeThatShouldBeIntercepted);
            var typeDefinition = TestInspector.FindType(typeToFind);
            
            Assert.That(typeDefinition, Is.Not.Null);
            Assert.That(typeDefinition.FullName, Is.EqualTo(typeToFind.FullName));
        }

        [Test]
        [ExpectedException(typeof(AssemblyInspectorTypeNotFoundException))]
        public void Should_throw_if_type_not_found()
        {
            var typeThatCannotBeFound = typeof (TypeThatCannotBeFound);
            TestInspector.FindType(typeThatCannotBeFound);
        }

        [Test]
        [ExpectedException(typeof(AssemblyInspectorAssemblyNotFoundException))]
        public void Should_throw_when_assembly_not_found()
        {
            new AssemblyInspector("Some assembly that does not exist");
        }
    }
}
