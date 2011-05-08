using System;
using System.IO;
using CryoAOP.Core;
using CryoAOP.Core.Exceptions;
using CryoAOP.TestAssembly;
using NUnit.Framework;
using Mono.Cecil;

namespace CryoAOP.Tests.Core
{
    [TestFixture]
    public class AssemblyTests
    {
        private Assembly assembly;
        const string assemblyName = "CryoAOP.TestAssembly.dll";

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            assembly = new Assembly(System.Reflection.Assembly.LoadFrom(assemblyName));
        }

        [Test]
        public void Should_find_expected_type_from_assembly()
        {
            var type = assembly.FindType(typeof (MethodInterceptorTarget));
            Assert.That(type, Is.Not.Null);
            
            var typeEqualToFirstType = assembly.FindType(typeof (MethodInterceptorTarget).FullName);
            Assert.That(typeEqualToFirstType, Is.Not.Null);
            
            Assert.That(type, Is.EqualTo(typeEqualToFirstType));
        }

        [Test]
        public void Should_write_assembly_back_to_disk_successfully()
        {
            Exception thatShouldNotBeThrown = null;
            string tempAssembly = "AssemblyTests.Should_write_assembly_back_to_disk_successfully.dll";
            try
            {
                assembly.Write(tempAssembly);
            }
            catch(Exception e)
            {
                thatShouldNotBeThrown = e;
            }
            finally
            {
                if (File.Exists(tempAssembly))
                    File.Delete(tempAssembly);
            }

            if (thatShouldNotBeThrown != null)
                throw thatShouldNotBeThrown;
        }

        [Test]
        [ExpectedException(typeof(TypeNotFoundException))]
        public void Should_throw_when_type_not_found()
        {
            assembly.FindType("That Does Not Exist");
        }

        [Test]
        public void ToString_should_return_definition_fullname()
        {
            Assert.That(assembly.ToString(), Is.EqualTo(assembly.Definition.FullName));
        }

        [Test]
        public void If_assemblies_are_the_same_they_should_be_equal()
        {
            var sameAssembly = new Assembly(System.Reflection.Assembly.LoadFrom(assemblyName));
            Assert.That(assembly, Is.EqualTo(sameAssembly));
        }

        [Test]
        public void Equality_operators_should_return_true_if_same_assembly()
        {
            var sameAssembly = new Assembly(System.Reflection.Assembly.LoadFrom(assemblyName));
            Assert.That(assembly == sameAssembly);
        }

        [Test]
        public void Equality_operators_should_return_false_if_same_assembly()
        {
            var sameAssembly = new Assembly(System.Reflection.Assembly.LoadFrom("CryoAOP.Tests.dll"));
            Assert.That(assembly != sameAssembly);
        }

        [Test]
        public void GetHashCode_should_be_generated_from_full_name()
        {
            Assert.That(assembly.GetHashCode(), Is.EqualTo(assembly.FullName.GetHashCode()));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Should_throw_if_argument_for_contructor_is_null()
        {
            new Assembly(null, null);
        }

        [Test]
        [ExpectedException(typeof(AssemblyNotFoundException))]
        public void Should_throw_if_assembly_not_found()
        {
            new Assembly("Some Assembly That Does Not Exist");
        }

        [Test]
        public void Equality_should_fail_if_right_is_null()
        {
            Assert.That(assembly.Equals((object) null), Is.False);
            Assert.That(assembly.Equals((Assembly)null), Is.False);
        }

        [Test]
        public void Equality_should_pass_if_right_is_same()
        {
            Assert.That(assembly.Equals(assembly), Is.True);
            Assert.That(assembly.Equals((object)assembly), Is.True);
        }

        [Test]
        public void Euality_should_return_false_if_types_are_not_the_same()
        {
            Assert.That(assembly.Equals((object)"Some other object"), Is.False);
        }

        public class TestAssembly : Assembly
        {
            public ReaderParameters ReaderParamsUsedForLoad = null;
            public TestAssembly(string assemblyPath, ReaderParameters @params) : base(assemblyPath, @params) { }
            public override void LoadAssemblyDefinition(ReaderParameters @params) { ReaderParamsUsedForLoad = @params; base.LoadAssemblyDefinition(@params); }
        }

        [Test]
        public void Assembly_load_params_should_be_passed_to_assembly_definition()
        {
            var assembly = new TestAssembly(assemblyName, AssemblyParams.ImmediateLoad);
            Assert.That(assembly.ReaderParamsUsedForLoad.ReadingMode, Is.EqualTo(ReadingMode.Immediate));
        }
    }
}