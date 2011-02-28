using System;
using System.Collections.Generic;

namespace CryoAOP
{
    public class AssemblyLine : ConfigLine
    {
        private readonly List<TypeLine> types;

        public AssemblyLine(int lineNumber, string line) : base(lineNumber, line)
        {
            types = new List<TypeLine>();
        }

        public string InputAssembly
        {
            get { return Value.Split(',')[0].Trim(); }
        }

        public string OutputAssembly
        {
            get { return Value.Contains(",") ? Value.Split(',')[1].Trim() : InputAssembly; }
        }

        public List<TypeLine> Types
        {
            get { return types; }
        }
        public bool HasTypes
        {
            get { return types != null && types.Count > 0; }
        }

        public static bool IsAssembly(string line)
        {
            return line.ToLower().StartsWith("assembly") && line.Contains(":");
        }


    }
}