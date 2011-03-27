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
using System.Linq;
using CryoAOP.Core.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CryoAOP.Core
{
    internal class MethodMarkerCacheItem
    {
        public readonly string TypeAndMethodName;
        public readonly bool HasMarker;

        public MethodMarkerCacheItem(string typeAndMethodName, bool hasMarker)
        {
            TypeAndMethodName = typeAndMethodName;
            HasMarker = hasMarker;
        }

        public bool Equals(MethodMarkerCacheItem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.TypeAndMethodName, TypeAndMethodName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (MethodMarkerCacheItem)) return false;
            return Equals((MethodMarkerCacheItem) obj);
        }

        public override int GetHashCode()
        {
            return (TypeAndMethodName != null ? TypeAndMethodName.GetHashCode() : 0);
        }
    }

    internal class MethodMarkerCache
    {
        public static Dictionary<int, MethodMarkerCacheItem> Cache = new Dictionary<int, MethodMarkerCacheItem>();
    }

    internal class MethodMarker
    {
        public virtual bool HasMarker(MethodDefinition method, string markerDefinition)
        {
            var methodTypeAndName = method.DeclaringType.FullName + method.Name;
            var methodNameHashCode = methodTypeAndName.GetHashCode();
            if (MethodMarkerCache.Cache.ContainsKey(methodNameHashCode))
                return MethodMarkerCache.Cache[methodNameHashCode].HasMarker;

            if (method == null
                || method.Body == null
                || method.Body.Instructions == null
                || method.Body.Instructions.Count <= 2)
                return false;

            var interceptMarker = method.Body.Instructions.ToList().Take(2).ToArray();
            var firstInstruction = interceptMarker.First();
            var hasMarker = 
                firstInstruction.OpCode == OpCodes.Ldstr
                && (firstInstruction.Operand as string) == markerDefinition;

            if (hasMarker)
                MethodMarkerCache.Cache.Add(methodNameHashCode, new MethodMarkerCacheItem(methodTypeAndName, hasMarker));
            return hasMarker;
        }

        public virtual void CreateMarker(MethodDefinition method, string markerDefinition)
        {
            var il = method.Body.GetILProcessor();
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldstr, markerDefinition),
                              il.Create(OpCodes.Pop)
                          });
        }
    }
}