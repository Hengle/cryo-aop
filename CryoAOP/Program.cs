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
using CryoAOP.Core;
using CryoAOP.Core.Extensions;
using CryoAOP.Exec;

namespace CryoAOP
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("CryoAOP -> Starting up!");
            if (args == null || args.Length == 0)
            {
                WriteUsage();
                return;
            }

            if (args.Any(a => a == "/nowarn"))
                ErrorExtensions.DisableWarnings = true;

            if (args.Any(a => a == "/aspects"))
                Intercept.InterceptAspects();

            if (args.Any(a => a == "/i"))
            {
                var inputFile = args.Where(param => param.ToLower().EndsWith(".cryoaop")).FirstOrDefault();
                if (inputFile == null)
                {
                    "CryoAOP -> Could not find input file parameter!".Error();
                    WriteUsage();
                    return;
                }

                if (!File.Exists(inputFile))
                {
                    "CryoAOP -> '{0}' does not exist!".Error(inputFile);
                    WriteUsage();
                    return;
                }

                var assemblyLines = ParseInputFile(inputFile);
                InterceptMethods(assemblyLines);
            }

            Environment.ExitCode = 0;
        }

        private static IEnumerable<AssemblyLine> ParseInputFile(string inputFile)
        {
            var currentLineCount = 0;
            var typeLines = new List<TypeLine>();
            var assemblyLines = new List<AssemblyLine>();
            using (var reader = new StreamReader(inputFile))
            {
                while (!reader.EndOfStream)
                {
                    currentLineCount++;
                    var currentLine = reader.ReadLine();

                    if (string.IsNullOrEmpty(currentLine)) continue;
                    if (currentLine.Trim() == "") continue;
                    if (currentLine.ToLower().Trim().Equals("break;")) break;
                    if (currentLine.ToLower().Trim().Equals("return;")) return assemblyLines;
                    if (!currentLine.ToLower().Trim().StartsWith("//"))
                    {
                        if (AssemblyLine.IsAssembly(currentLine))
                        {
                            var assemblyLine = new AssemblyLine(currentLineCount, currentLine);
                            assemblyLines.Add(assemblyLine);
                        }
                        else if (TypeLine.IsType(currentLine))
                        {
                            if (assemblyLines.Count == 0)
                            {
                                "CryoAOP -> Error:{0}! Could not find matching assembly tag for type ... ".Error(currentLineCount.ToString());
                                "CryoAOP -> '{0}'".Error(currentLine.Trim());
                                WriteUsage();
                                return assemblyLines;
                            }

                            var typeLine = new TypeLine(currentLineCount, currentLine);
                            assemblyLines.Last().Types.Add(typeLine);
                            typeLines.Add(typeLine);
                        }
                        else if (MethodLine.IsMethod(currentLine))
                        {
                            if (assemblyLines.Count == 0 && typeLines.Count == 0)
                            {
                                "CryoAOP -> Error:{0}! Could not find matching type tag for method ... ".Error(currentLineCount.ToString());
                                "CryoAOP -> '{0}'".Error(currentLine.Trim());
                                WriteUsage();
                                return assemblyLines;
                            }

                            var methodLine = new MethodLine(currentLineCount, currentLine);
                            typeLines.Last().Methods.Add(methodLine);
                        }
                        else if (PropertyLine.IsProperty(currentLine))
                        {
                            if (assemblyLines.Count == 0 && typeLines.Count == 0)
                            {
                                "CryoAOP -> Error:{0}! Could not find matching type tag for property ... ".Error(currentLineCount.ToString());
                                "CryoAOP -> '{0}'".Error(currentLine.Trim());
                                WriteUsage();
                                return assemblyLines;
                            }

                            var propertyLine = new PropertyLine(currentLineCount, currentLine);
                            typeLines.Last().Properties.Add(propertyLine);
                        }
                        else
                        {
                            "CryoAOP -> Warning:{0}! Ignoring line because entry appears to be invalid ... ".Warn(currentLineCount);
                            "CryoAOP -> '{0}'".Warn(currentLine.Trim());
                        }
                    }
                }
            }
            return assemblyLines;
        }

        private static void InterceptMethods(IEnumerable<AssemblyLine> assemblyLines)
        {
            foreach (var assembly in assemblyLines)
            {
                Intercept.LoadAssembly(assembly.InputAssembly);

                if (assembly.HasTypes)
                {
                    foreach (var type in assembly.Types)
                    {
                        if (type.HasMethods)
                        {
                            foreach (var method in type.Methods)
                            {
                                Intercept.InterceptMethod(type.FullTypeName, method.MethodName, method.MethodScope);
                                Console.WriteLine(
                                    "CryoAOP -> Intercepted {0}\\{1}\\{2}"
                                    .FormatWith(
                                        assembly.InputAssembly,
                                        type.FullTypeName,
                                        method.MethodName));
                            }
                        }
                        else if (type.HasProperties)
                        {
                            foreach (var property in type.Properties)
                            {
                                Intercept.InterceptProperty(type.FullTypeName, property.PropertyName, property.MethodScope);
                                Console.WriteLine(
                                    "CryoAOP -> Intercepted {0}\\{1}\\{2}"
                                    .FormatWith(
                                        assembly.InputAssembly,
                                        type.FullTypeName,
                                        property.PropertyName));
                            }
                        }
                        else
                        {
                            Intercept.InterceptType(type.FullTypeName, type.MethodScope);
                            Console.WriteLine(
                                "CryoAOP -> Intercepted {0}\\{1}\\*"
                                .FormatWith(
                                    assembly.InputAssembly,
                                    type.FullTypeName));
                        }
                    }
                }
                else
                {
                    Intercept.InterceptAll(assembly.MethodScope);
                    Console.WriteLine("CryoAOP -> Intercepted {0}\\*\\*".FormatWith(assembly.InputAssembly));
                }

                Intercept.SaveAssembly(assembly.OutputAssembly);
            }
            Console.WriteLine("CryoAOP -> Finished!");
        }

        private static void WriteUsage()
        {
            Console.WriteLine("CryoAOP v1.0 by fir3pho3nixx");
            Console.WriteLine("Usage: CryoAOP /input input.cryoaop /nowarn");
            Console.WriteLine("Where: ");
            Console.WriteLine("     /input <inputfile.cryoaop> -> is an input file with an extension of '*.cryoaop'.");
            Console.WriteLine("     /nowarn                    -> is when we do not want to see warnings for assembly load failures.");
            Console.WriteLine("     /apsects                   -> find 'Intercept' and 'Mixin' attributes.");
            Console.WriteLine();
            Console.WriteLine("Example: Input File -> The scope to the interception can be set for 'Assembly', 'Type' or 'Method'.");
            Console.WriteLine("Assembly: InputAssemblyName.dll, OutputAssemblyName.dll");
            Console.WriteLine("   Type: ExampleNamespace.ExampleClassToIntercept");
            Console.WriteLine("      Method: MethodToIntercept(); /* Where Scope is defaulted to shallow */");
            Console.WriteLine("      Method<shallow>: MethodToIntercept(int); /* Only intercepts external calls to member */");
            Console.WriteLine("      Method<deep>: MethodToIntercept(string,int); /* Intercepts internal calls to member within assembly, incurs performance hit */");
        }
    }
}