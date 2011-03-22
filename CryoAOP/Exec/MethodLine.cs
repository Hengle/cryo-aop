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

using System.Collections.Generic;

namespace CryoAOP.Exec
{
    internal class MethodLine : ConfigLine
    {
        public MethodLine(int lineNumber, string line) : base(lineNumber, line)
        {
        }

        public string MethodName
        {
            get
            {
                return Value.Split('(')[0].Trim();
            }
        }

        public string[] MethodParameterTypes
        {
            get
            {
                var types = new List<string>();
                foreach (var type in Value.Replace(MethodName, "").Trim().Replace("(", "").Replace(")", "").Replace(";", "").Split(','))
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