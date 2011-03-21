namespace CryoAOP.Exec
{
    internal class PropertyLine : ConfigLine
    {
        public PropertyLine(int lineNumber, string line)
            : base(lineNumber, line)
        {
        }

        public string PropertyName
        {
            get { return Value.Trim(); }
        }

        public static bool IsProperty(string currentLine)
        {
            return currentLine.ToLower().Trim().StartsWith("property") && currentLine.Contains(":");
        }
    }
}