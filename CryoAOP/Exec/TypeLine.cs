using System.Collections.Generic;

namespace CryoAOP.Exec
{
    public class TypeLine : ConfigLine
    {
        private readonly List<MethodLine> methods;
        private readonly List<PropertyLine> properties;

        public TypeLine(int lineNumber, string line) : base(lineNumber, line)
        {
            methods = new List<MethodLine>();
            properties = new List<PropertyLine>();
        }

        public string FullTypeName
        {
            get { return Value.Trim(); }
        }

        public List<MethodLine> Methods
        {
            get { return methods; }
        }

        public List<PropertyLine> Properties
        {
            get { return properties; }
        }

        public bool HasMethods
        {
            get { return methods != null && methods.Count > 0; }
        }

        public bool HasProperties
        {
            get { return properties != null && properties.Count > 0; }
        }

        public static bool IsType(string currentLine)
        {
            return currentLine.ToLower().Trim().StartsWith("type") && currentLine.Contains(":");
        }
    }
}