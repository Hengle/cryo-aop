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
    internal class TypeLine : ConfigLine
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