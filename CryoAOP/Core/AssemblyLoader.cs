using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CryoAOP.Core
{
    internal class ShadowAssemblyType
    {
        public readonly Assembly ShadowAssembly;
        public readonly string OriginalAssemblyPath;

        public ShadowAssemblyType(Assembly shadowAssembly, string originalAssemblyPath)
        {
            ShadowAssembly = shadowAssembly;
            OriginalAssemblyPath = originalAssemblyPath;
        }
    }

    internal class AssemblyLoader
    {
        public static List<ShadowAssemblyType> Assemblies = new List<ShadowAssemblyType>();

        public ShadowAssemblyType[] GetShadowAssemblies()
        {
            if (Assemblies.Count == 0)
            {
                if (!Directory.Exists("C:\\Temp"))
                    Directory.CreateDirectory("C:\\Temp");

                var fileList = Directory
                    .GetFiles(Environment.CurrentDirectory)
                    .Where(f =>
                               {
                                   var lowerCaseFileName = f.ToLower();
                                   return (lowerCaseFileName.EndsWith(".dll") || lowerCaseFileName.EndsWith(".exe"))
                                          && lowerCaseFileName.IndexOf("cryoaop.exe") == -1
                                          && lowerCaseFileName.IndexOf("cryoaop.aspects.dll") == -1
                                          && lowerCaseFileName.IndexOf("mono.cecil.dll") == -1;
                               }
                    )
                    .ToList();

                foreach (var file in fileList)
                {
                    var temporaryFile = "C:\\Temp\\" + Path.GetFileName(file);
                    if (File.Exists(temporaryFile))
                        File.Delete(temporaryFile);

                    File.Copy(file, temporaryFile);
                    var assembly = Assembly.LoadFrom(temporaryFile);

                    var shadow = new ShadowAssemblyType(assembly, file);
                    Assemblies.Add(shadow);
                }
            }
            return Assemblies.ToArray();
        }
    }
}