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

using CryoAOP.Core.Factories;
using Mono.Cecil;

namespace CryoAOP.Core
{
    internal class MethodContext
    {
        public readonly Type Type;
        public readonly MethodDefinition MethodDefinition;
        public readonly PropertyDefinition PropertyDefinition;

        public readonly ImporterFactory Importer;
        public readonly MethodCloneFactory Cloning;
        public readonly NameAliasFactory NameAlias;
        
        public readonly Method Method;
        public readonly Property Property;

        public readonly MethodMarker Marker;
        public readonly MethodMixinExtension Mixin;
        public readonly MethodScopingExtension Scope;

        public MethodContext(Type type, Method method, MethodDefinition methodDefinition)
        {
            Type = type;
            Method = method;
            MethodDefinition = methodDefinition;
            Marker = new MethodMarker();
            NameAlias = new NameAliasFactory();
            Importer = new ImporterFactory(this);
            Cloning = new MethodCloneFactory(this);
            Mixin = new MethodMixinExtension(this);
            Scope = new MethodScopingExtension(this);
        }

        public MethodContext(Type type, Property method, PropertyDefinition methodDefinition)
        {
            Type = type;
            Property = method;
            PropertyDefinition = methodDefinition;
            Marker = new MethodMarker();
            NameAlias = new NameAliasFactory();
            Importer = new ImporterFactory(this);
            Cloning = new MethodCloneFactory(this);
            Mixin = new MethodMixinExtension(this);
            Scope = new MethodScopingExtension(this);
        }

        public Assembly Assembly
        {
            get { return Type.Assembly; }
        }

        public AssemblyDefinition AssemblyDefinition
        {
            get { return Assembly.Definition; }
        }

        public TypeDefinition TypeDefinition
        {
            get { return Type.Definition; }
        }
    }
}