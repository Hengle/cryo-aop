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

            Debugger.Launch();

            var currentLineCount = 0;
            var typeLines = new List<TypeLine>();
            var assemblyLines = new List<AssemblyLine>();
            using (var reader = new StreamReader(inputFile))
            {
                while (!reader.EndOfStream)
                {
                    currentLineCount++;
                    var currentLine = reader.ReadLine();

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

            foreach (var type in assemblyLines[0].Types)
            {
                foreach (var method in type.Methods)
                {
                    var s = method.Scope;
                }
            }
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