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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using CryoAOP.Core.Exceptions;
using CryoAOP.Core.Extensions;
using Mono.Cecil;

namespace CryoAOP.Core
{
    internal class Assembly
    {
        public readonly AssemblyDefinition Definition;
        private readonly string assemblyPath = "";


        public Assembly(System.Reflection.Assembly assembly)
            : this(assembly
                       .CodeBase
                       .Replace("file:///", "")
                       .Replace("/", @"\"),
                   AssemblyParams
                       .DeferredLoad)
        {
        }

        public Assembly(string assemblyPath, ReaderParameters @params = null)
        {
            if (@params == null)
                @params = AssemblyParams.ReadSymbols;

            if (assemblyPath == null) throw new ArgumentNullException("assemblyPath");
            this.assemblyPath = assemblyPath;

            try
            {
                Definition =
                    AssemblyDefinition
                        .ReadAssembly(
                            this.assemblyPath,
                            @params);

                AssemblyParams
                    .AssemblyResolver
                    .AddSearchDirectory(
                        RuntimeEnvironment
                            .GetRuntimeDirectory());
            }
            catch (Exception err)
            {
                throw new AssemblyNotFoundException(
                    "Could not load assembly from '{0}'", err, assemblyPath);
            }
        }

        public virtual Type FindType(string searchType)
        {
            foreach (var module in Definition.Modules)
            {
                foreach (var type in module.Types)
                {
                    if (type.FullName.ToLower().EndsWith(searchType.ToLower()))
                        return new Type(this, type);
                }
            }

            throw new TypeNotFoundException(
                "Could not find type for '{0}' in assembly '{1}'",
                searchType, Path.GetFileName(assemblyPath));
        }

        public virtual Type FindType(System.Type searchType)
        {
            return FindType(searchType.FullName);
        }

        public virtual void Write(string path)
        {
            Definition.Write(path, AssemblyParams.WriterParameters);
        }

        public override string ToString()
        {
            return "{0}".FormatWith(Definition.FullName);
        }
    }
}