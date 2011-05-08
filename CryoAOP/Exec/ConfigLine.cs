using CryoAOP.Core;

namespace CryoAOP.Exec
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
                return "deep";
            }
        }

        public MethodInterceptionScopeType MethodScope
        {
            get
            {
                if (Scope.ToLower() == "shallow")
                    return MethodInterceptionScopeType.Shallow;
                if (Scope.ToLower() == "deep")
                    return MethodInterceptionScopeType.Deep;

                "Could not resolve interception scope! Defaulting to 'shallow' ... ".Error(LineNumber);
                Line.Error();

                return MethodInterceptionScopeType.Deep;
            }
        }
    }
}