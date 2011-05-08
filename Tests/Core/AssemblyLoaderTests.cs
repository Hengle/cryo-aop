using System;
using System.IO;
using System.Linq;
using CryoAOP.Core;
using CryoAOP.Core.Extensions;
using NUnit.Framework;
using Rhino.Mocks;

namespace CryoAOP.Tests.Core
{
    [TestFixture]
    public class AssemblyLoaderTests
    {
        private IAssemblyLoader assemblyLoader;
        private ShadowAssemblyType[] shadowAssemblies;

        [TestFixtureSetUp]
        public void SetUp()
        {
            const string tempDir = @"c:\temp";

            Directory.GetFiles(tempDir).ToList().ForEach(File.Delete);

            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir);
            
            assemblyLoader = new AssemblyLoader();
            shadowAssemblies = assemblyLoader.GetShadowAssemblies();
        }

        [Test]
        public void Loader_should_create_a_temp_directory_if_one_does_not_exist()
        {
            Assert.That(Directory.Exists(@"c:\temp"));
        }

        [Test]
        public void Loader_should_register_a_test_assembly()
        {
            Assert.That(shadowAssemblies.Any(a => a.ShadowAssembly.FullName.IndexOf("CryoAOP.TestAssembly") != -1));
        }

        [Test]
        public void Loader_should_only_load_files_from_temp_directory_to_avoid_file_locking()
        {
            var testAssembly = shadowAssemblies.First(a => a.ShadowAssembly.FullName.IndexOf("CryoAOP.TestAssembly") != -1);
            Assert.That(testAssembly.ShadowAssembly.Location.ToLower(), Is.EqualTo(@"c:\temp\cryoaop.testassembly.dll"));
        }

        [Test]
        public void Should_not_load_assemblies_more_than_once()
        {
            Assert.That(AssemblyLoader.HasRun);
            var newAssemblyLoader = MockRepository.GeneratePartialMock<AssemblyLoader>();
            newAssemblyLoader.GetShadowAssemblies();
            newAssemblyLoader.AssertWasNotCalled(l => l.LoadShadowAssemblies());
        }

        [Test]
        public void Loader_should_overwrite_files_if_they_exist_in_temp_already()
        {
            const string file = "CryoAOP.TestAssembly.dll";
            var tempFile = @"c:\temp\{0}_CryoAOP.TestAssembly.dll".FormatWith(Guid.NewGuid().ToString("N"));
            File.Copy(file, tempFile);
            Assert.DoesNotThrow(() =>  new AssemblyLoader().EnsureTempFileIsCopied(file, tempFile));
        }

        [Test]
        public void Shadow_assemblies_should_implement_to_string_using_fullname()
        {
            foreach (var shadowAssembly in shadowAssemblies)
                Assert.That(shadowAssembly.ToString(), Is.EqualTo(shadowAssembly.ShadowAssembly.FullName));
        }
    }
}