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