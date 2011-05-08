using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CryoAOP.Core
{
    public class ShadowAssemblyType
    {
        public readonly string OriginalAssemblyPath;
        public readonly System.Reflection.Assembly ShadowAssembly;

        public ShadowAssemblyType(System.Reflection.Assembly shadowAssembly, string originalAssemblyPath)
        {
            ShadowAssembly = shadowAssembly;
            OriginalAssemblyPath = originalAssemblyPath;
        }

        public override string ToString()
        {
            return string.Format("{0}", ShadowAssembly.FullName);
        }
    }

    public interface IAssemblyLoader
    {
        ShadowAssemblyType[] GetShadowAssemblies();
    }

    public class AssemblyLoader : IAssemblyLoader
    {
        public static bool HasRun;
        public static ShadowAssemblyType[] AssembliesFromPreviousRun;
        public static List<ShadowAssemblyType> Assemblies = new List<ShadowAssemblyType>();

        public IEnumerable<string> RestrictedAssemblies
        {
            get { return new[] {"cryoaop.exe", "cryoaop.aspects.dll", "mono.cecil.dll", "mono.cecil.pdb.dll"}; }
        }

        public virtual ShadowAssemblyType[] GetShadowAssemblies()
        {
            if (HasRun)
                return AssembliesFromPreviousRun;
            return LoadShadowAssemblies();
        }

        public virtual ShadowAssemblyType[] LoadShadowAssemblies()
        {
            if (Assemblies.Count == 0)
            {
                LoadAssembliesFromTemp();
            }

            var shadowAssemblyTypes = Assemblies.ToArray();
            AssembliesFromPreviousRun = shadowAssemblyTypes;
            HasRun = true;

            return shadowAssemblyTypes;
        }

        public virtual void LoadAssembliesFromTemp()
        {
            if (!Directory.Exists("C:\\Temp"))
                Directory.CreateDirectory("C:\\Temp");

            var fileList = GetFilteredFileList();
            foreach (var file in fileList)
            {
                System.Reflection.Assembly assembly = LoadAssemblyFromTemp(file);
                var shadow = new ShadowAssemblyType(assembly, file);
                Assemblies.Add(shadow);
            }
        }

        public virtual List<string> GetFilteredFileList()
        {
            return Directory
                .GetFiles(AppDomain.CurrentDomain.BaseDirectory)
                .Where(f =>
                           {
                               var lowerCaseFileName = f.ToLower();
                               return (lowerCaseFileName.EndsWith(".dll") || lowerCaseFileName.EndsWith(".exe"))
                                      && RestrictedAssemblies.All(ra => lowerCaseFileName.IndexOf(ra) == -1);
                           }
                )
                .ToList();
        }

        public virtual System.Reflection.Assembly LoadAssemblyFromTemp(string file)
        {
            var temporaryFile = "C:\\Temp\\" + Path.GetFileName(file);
            EnsureTempFileIsCopied(file, temporaryFile);
            return System.Reflection.Assembly.LoadFrom(temporaryFile);
        }

        public virtual void EnsureTempFileIsCopied(string file, string temporaryFile)
        {
            if (File.Exists(temporaryFile))
                File.Delete(temporaryFile);
            File.Copy(file, temporaryFile);
        }
    }
}