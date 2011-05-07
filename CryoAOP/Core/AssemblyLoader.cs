using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CryoAOP.Core
{
    internal class ShadowAssemblyType
    {
        public readonly string OriginalAssemblyPath;
        public readonly System.Reflection.Assembly ShadowAssembly;

        public ShadowAssemblyType(System.Reflection.Assembly shadowAssembly, string originalAssemblyPath)
        {
            ShadowAssembly = shadowAssembly;
            OriginalAssemblyPath = originalAssemblyPath;
        }
    }

    internal class AssemblyLoader
    {
        public static bool HasRun;
        public static ShadowAssemblyType[] AssembliesFromPreviousRun;
        public static List<ShadowAssemblyType> Assemblies = new List<ShadowAssemblyType>();

        public IEnumerable<string> RestrictedAssemblies
        {
            get { return new[] {"cryoaop.exe", "cryoaop.aspects.dll", "mono.cecil.dll", "mono.cecil.pdb.dll"}; }
        }

        public ShadowAssemblyType[] GetShadowAssemblies()
        {
            if (HasRun)
                return AssembliesFromPreviousRun;

            if (Assemblies.Count == 0)
            {
                if (!Directory.Exists("C:\\Temp"))
                    Directory.CreateDirectory("C:\\Temp");

                var fileList = Directory
                    .GetFiles(AppDomain.CurrentDomain.BaseDirectory)
                    .Where(f =>
                               {
                                   var lowerCaseFileName = f.ToLower();
                                   return (lowerCaseFileName.EndsWith(".dll") || lowerCaseFileName.EndsWith(".exe"))
                                          && RestrictedAssemblies.All(ra => lowerCaseFileName.IndexOf(ra) == -1);
                               }
                    )
                    .ToList();

                foreach (var file in fileList)
                {
                    var temporaryFile = "C:\\Temp\\" + Path.GetFileName(file);
                    if (File.Exists(temporaryFile))
                        File.Delete(temporaryFile);

                    File.Copy(file, temporaryFile);
                    var assembly = System.Reflection.Assembly.LoadFrom(temporaryFile);

                    var shadow = new ShadowAssemblyType(assembly, file);
                    Assemblies.Add(shadow);
                }
            }

            var shadowAssemblyTypes = Assemblies.ToArray();
            AssembliesFromPreviousRun = shadowAssemblyTypes;
            HasRun = true;

            return shadowAssemblyTypes;
        }
    }
}