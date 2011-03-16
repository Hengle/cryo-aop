using CryoAOP.Core.Factories;
using Mono.Cecil;

namespace CryoAOP.Core.Methods
{
    internal class MethodContext
    {
        public readonly Type Type;
        public readonly MethodDefinition Definition;

        public readonly ImporterFactory Importer;
        public readonly MethodCloneFactory Cloning;
        public readonly NameAliasFactory NameAlias;
        
        public readonly Method Method;
        public readonly MethodMarker Marker;
        public readonly MethodMixinExtension Mixin;
        public readonly MethodScopingExtension Scope;

        public MethodContext(Type type, Method method, MethodDefinition definition)
        {
            Type = type;
            Method = method;
            Definition = definition;
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