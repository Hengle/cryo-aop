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

using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CryoAOP.Core
{
    internal class MethodScopingExtension : MethodExtension
    {
        public MethodScopingExtension(MethodContext method) : base(method)
        {
        }

        public void ModifyCallScope(MethodDefinition renamedMethod, MethodDefinition interceptorMethod, ILProcessor il, MethodInterceptionScopeType interceptionScope)
        {
            if (interceptionScope == MethodInterceptionScopeType.Deep)
            {
                foreach (var module in Type.Assembly.Definition.Modules)
                {
                    foreach (var type in module.Types.ToList())
                    {
                        if (type.Methods == null || type.Methods.Count == 0) continue;
                        foreach (var method in type.Methods.ToList())
                        {
                            if (Context.Marker.HasMarker(method, Method.MethodMarker)) continue;

                            if (method == null
                                || method.Body == null
                                || method.Body.Instructions == null
                                || method.Body.Instructions.Count() == 0)
                                continue;

                            foreach (var instruction in method.Body.Instructions.ToList())
                            {
                                if (instruction.OpCode == OpCodes.Call && instruction.Operand == renamedMethod)
                                {
                                    var processor = method.Body.GetILProcessor();
                                    processor.InsertAfter(instruction, il.Create(OpCodes.Call, interceptorMethod));
                                    processor.Remove(instruction);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
