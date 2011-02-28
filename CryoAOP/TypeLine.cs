using System;
using System.Collections.Generic;

namespace CryoAOP
{
    public class TypeLine : ConfigLine
    {
        private readonly List<MethodLine> methods;

        public TypeLine(int lineNumber, string line) : base(lineNumber, line)
        {
            methods = new List<MethodLine>();
        }

        public string FullTypeName
        {
            get { return Value.Trim(); }
        }

        public List<MethodLine> Methods
        {
            get { return methods; }
        }
        public bool HasMethods
        {
            get { return methods != null && methods.Count > 0; }
        }

        public static bool IsType(string currentLine)
        {
            return currentLine.ToLower().Trim().StartsWith("type") && currentLine.Contains(":");
        }
    }
}