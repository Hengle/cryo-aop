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

using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;

namespace CryoAOP.Core
{
    internal class AssemblyParams
    {
        public static DefaultAssemblyResolver AssemblyResolver
        {
            get { return (DefaultAssemblyResolver) GlobalAssemblyResolver.Instance; }
        }

        public static ISymbolWriterProvider SymbolWriterProvider
        {
            get { return new PdbWriterProvider(); }
        }

        public static WriterParameters WriterParameters
        {
            get
            {
                var @params = new WriterParameters();
                @params.WriteSymbols = true;
                @params.SymbolWriterProvider = SymbolWriterProvider;
                return @params;
            }
        }

        public static ISymbolReaderProvider SymbolReaderProvider
        {
            get { return new PdbReaderProvider(); }
        }

        public static ReaderParameters ReadSymbols
        {
            get
            {
                var @params = new ReaderParameters();
                @params.ReadSymbols = true;
                @params.SymbolReaderProvider = SymbolReaderProvider;
                return @params;
            }
        }

        public static ReaderParameters DeferredLoad
        {
            get
            {
                var @params = new ReaderParameters(ReadingMode.Deferred);
                @params.ReadSymbols = false;
                return @params;
            }
        }

        public static ReaderParameters ImmediateLoad
        {
            get
            {
                var @params = new ReaderParameters(ReadingMode.Immediate);
                @params.ReadSymbols = false;
                return @params;
            }
        }
    }
}