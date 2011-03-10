﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using CryoAOP.Core.Exceptions;
using CryoAOP.Core.Extensions;
using Mono.Cecil;

namespace CryoAOP.Core
{
    internal class AssemblyIntercept
    {
        public readonly AssemblyDefinition Definition;
        private readonly string assemblyPath = "";


        public AssemblyIntercept(Assembly assembly)
            : this(assembly
                       .CodeBase
                       .Replace("file:///", "")
                       .Replace("/", @"\"),
                   AssemblyInterceptParams
                       .DeferredLoad)
        {
        }

        public AssemblyIntercept(string assemblyPath, ReaderParameters @params = null)
        {
            if (@params == null)
                @params = AssemblyInterceptParams.ReadSymbols;

            if (assemblyPath == null) throw new ArgumentNullException("assemblyPath");
            this.assemblyPath = assemblyPath;

            try
            {
                Definition =
                    AssemblyDefinition
                        .ReadAssembly(
                            this.assemblyPath,
                            @params);

                AssemblyInterceptParams
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

        public virtual TypeIntercept FindType(string searchType)
        {
            foreach (var module in Definition.Modules)
            {
                foreach (var type in module.Types)
                {
                    if (type.FullName.ToLower().EndsWith(searchType.ToLower()))
                        return new TypeIntercept(this, type);
                }
            }

            throw new TypeNotFoundException(
                "Could not find type for '{0}' in assembly '{1}'",
                searchType, Path.GetFileName(assemblyPath));
        }

        public virtual TypeIntercept FindType(Type searchType)
        {
            return FindType(searchType.FullName);
        }

        public virtual void Write(string path)
        {
            Definition.Write(path, AssemblyInterceptParams.WriterParameters);
        }

        public override string ToString()
        {
            return "{0}".FormatWith(Definition.FullName);
        }
    }
}