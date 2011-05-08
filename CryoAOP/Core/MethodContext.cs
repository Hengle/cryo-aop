using CryoAOP.Core.Factories;
using Mono.Cecil;

namespace CryoAOP.Core
{
    public class MethodContext
    {
        public readonly AssemblyFactory Assemblies;
        public readonly MethodCloneFactory Cloning;
        public readonly ImporterFactory Importer;
        public readonly MethodMarker Marker;
        public readonly Method Method;
        public readonly MethodDefinition MethodDefinition;
        public readonly MethodMixinExtension Mixin;
        public readonly NameAliasFactory NameAlias;
        public readonly Property Property;
        public readonly PropertyDefinition PropertyDefinition;
        public readonly MethodScopingExtension Scope;
        public readonly Type Type;

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
            Assemblies = new AssemblyFactory();
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
            Assemblies = new AssemblyFactory();
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