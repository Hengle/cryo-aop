using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CryoAOP.Core
{
    public class AssemblyLoader
    {
        public static List<Assembly> Assemblies = new List<Assembly>();

        public static Assembly[] GetShadowAssemblies()
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
                                   return (lowerCaseFileName.EndsWith(".dll")|| lowerCaseFileName.EndsWith(".exe"))
                                          && lowerCaseFileName.IndexOf("cryoaop.exe") == -1
                                          && lowerCaseFileName.IndexOf("mono.cecil.dll") == -1
                                          && lowerCaseFileName.IndexOf("mono.cecil.pdb.dll") == -1;
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
                    Assemblies.Add(assembly);
                }
            }
            return Assemblies.ToArray();
        }
    }
}