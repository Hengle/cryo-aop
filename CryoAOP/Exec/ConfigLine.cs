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

using CryoAOP.Aspects;

namespace CryoAOP.Exec
{
    internal class ConfigLine
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

                return MethodInterceptionScopeType.Shallow;
            }
        }
    }
}