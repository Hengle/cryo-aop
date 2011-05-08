using System.Collections.Generic;

namespace CryoAOP.Exec
{
    public class MethodLine : ConfigLine
    {
        public MethodLine(int lineNumber, string line) : base(lineNumber, line)
        {
        }

        public string MethodName
        {
            get { return Value.Split('(')[0].Trim(); }
        }

        public string[] MethodParameterTypes
        {
            get
            {
                var types = new List<string>();
                foreach (
                    var type in
                        Value.Replace(MethodName, "").Trim().Replace("(", "").Replace(")", "").Replace(";", "").Split(
                            ','))
                    if (type != null && type.Trim() != "")
                        types.Add(type.Trim());
                return types.ToArray();
            }
        }

        public static bool IsMethod(string currentLine)
        {
            return currentLine.ToLower().Trim().StartsWith("method") && currentLine.Contains(":");
        }
    }
}