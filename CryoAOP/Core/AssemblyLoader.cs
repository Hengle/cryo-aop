//CryoAOP. Aspect Oriented Framework for .NET.
//Copyright (C) 2011  Gavin van der Merwe (fir3pho3nixx@gmail.com)

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static bool HasRun = false;
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