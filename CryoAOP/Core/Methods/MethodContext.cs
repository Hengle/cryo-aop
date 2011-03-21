using CryoAOP.Core.Factories;
using CryoAOP.Core.Properties;
using Mono.Cecil;

namespace CryoAOP.Core.Methods
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