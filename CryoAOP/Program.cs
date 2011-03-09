using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CryoAOP.Core;
using CryoAOP.Core.Extensions;

namespace CryoAOP
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Debugger.Launch();

            if (args == null || args.Length == 0)
            {
                WriteUsage();
                return;
            }

            var inputFile = args.Where(param => param.ToLower().EndsWith(".cryoaop")).FirstOrDefault();
            if (inputFile == null)
            {
                "Could not find input file parameter!".Error();
                WriteUsage();
                return;
            }

            if (!File.Exists(inputFile))
            {
                "'{0}' does not exist!".Error(inputFile);
                WriteUsage();
                return;
            }

            var currentLineCount = 0;
            var typeLines = new List<TypeLine>();
            var assemblyLines = new List<AssemblyLine>();
            using (var reader = new StreamReader(inputFile))
            {
                while (!reader.EndOfStream)
                {
                    currentLineCount++;
                    var currentLine = reader.ReadLine();

                    if (currentLine.ToLower().Trim().Equals("break;")) break;
                    if (currentLine.ToLower().Trim().Equals("return;")) return;
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
                                "Error! Could not find matching assembly tag for type ... ".Error(currentLineCount);
                                "{0}".Error(currentLineCount, currentLine);
                                WriteUsage();
                                return;
                            }

                            var typeLine = new TypeLine(currentLineCount, currentLine);
                            assemblyLines.Last().Types.Add(typeLine);
                            typeLines.Add(typeLine);
                        }
                        else if (MethodLine.IsMethod(currentLine))
                        {
                            if (assemblyLines.Count == 0 && typeLines.Count == 0)
                            {
                                "Error! Could not find matching type tag for method ... ".Error(currentLineCount);
                                "{0}".Error(currentLineCount, currentLine);
                                WriteUsage();
                                return;
                            }

                            var methodLine = new MethodLine(currentLineCount, currentLine);
                            typeLines.Last().Methods.Add(methodLine);
                        }
                        else
                        {
                            "Warning! Ignoring line because entry appears to be invalid ... ".Error(currentLineCount);
                            "{0}".FormatWith(currentLineCount, currentLine);
                        }
                    }
                }
            }

            foreach(var assembly in assemblyLines)
            {
                Intercept.LoadAssembly(assembly.InputAssembly);

                if (assembly.HasTypes)
                {
                    foreach(var type in assembly.Types)
                    {
                        if (type.HasMethods)
                        {
                            foreach (var method in type.Methods)
                            {
                                Intercept.InterceptMethod(type.FullTypeName, method.MethodName, method.MethodScope);
                                Console.WriteLine("CryoAOP -> Intercepted {0}\\{1}\\{2}".FormatWith(assembly.InputAssembly, type.FullTypeName, method.MethodName));
                            }
                        }
                        else
                        {
                            Intercept.InterceptType(type.FullTypeName, type.MethodScope);
                            Console.WriteLine("CryoAOP -> Intercepted {0}\\{1}\\*".FormatWith(assembly.InputAssembly, type.FullTypeName));
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
            Console.WriteLine("Usage: CryoAOP /i input.cryoaop");
            Console.WriteLine("Where: /i input.cryoaop is of the following format.");
            Console.WriteLine();
            Console.WriteLine("Example: The scope to the interception can be set for 'Assembly', 'Type' or 'Method'.");
            Console.WriteLine("Assembly: FooAssembly.dll, OutFooAssembly.dll");
            Console.WriteLine("   Type: Foo.TypeToIntercept");
            Console.WriteLine("      Method: MethodToIntercept(); /* Where Scope is defaulted to shallow */");
            Console.WriteLine("      Method<shallow>: MethodToIntercept(int); /* Only intercepts external calls to member */");
            Console.WriteLine("      Method<deep>: MethodToIntercept(string,int); /* Intercepts internal calls to member within assembly, incurs performance hit */");
        }
    }
}