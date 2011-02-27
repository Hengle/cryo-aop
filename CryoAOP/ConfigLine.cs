using CryoAOP.Core;

namespace CryoAOP
{
    public class ConfigLine
    {
        protected readonly string Line;
        protected readonly int LineNumber;

        public ConfigLine(int lineNumber, string line)
        {
            Line = line;
            LineNumber = lineNumber;
        }

        public string Tag
        {
            get { return Line.Split(':')[0].Split('<')[0]; }
        }

        public string Value
        {
            get { return Line.Split(':')[1]; }
        }

        public string Scope
        {
            get
            {
                if (Line.Contains("<") && Line.Contains(">"))
                {
                    var scope = Line.Split(':')[0].Split('<')[1];
                    scope = scope.Split('>')[0];
                    return scope;
                }
                return "shallow";
            }
        }

        public MethodInterceptionScope MethodScope
        {
            get
            {
                if (Scope.ToLower() == "shallow")
                    return MethodInterceptionScope.Shallow;
                if (Scope.ToLower() == "deep")
                    return MethodInterceptionScope.Deep;

                "Could not resolve interception scope! Defaulting to 'shallow' ... ".Error(LineNumber);
                Line.Error();

                return MethodInterceptionScope.Shallow;
            }
        }
    }
}