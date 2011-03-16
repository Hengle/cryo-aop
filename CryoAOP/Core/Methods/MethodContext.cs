using CryoAOP.Core.Factories;
using Mono.Cecil;

namespace CryoAOP.Core.Methods
{
    internal class MethodContext
    {
        public readonly MethodDefinition Definition;
        public readonly Type Type;
        
        public readonly MethodCloneFactory CloneFactory;
        public readonly NameAliasFactory NameAliasFactory;
        public readonly ImporterFactory ImporterFactory;
        
        public readonly Method Method;
        public readonly MethodMarker MethodMarker;
        public readonly MethodMixinExtension MethodMixin;
        public readonly MethodScopingExtension MethodScope;

        public MethodContext(Type type, Method method, MethodDefinition definition)
        {
            // Intercepts/Definitions
            Definition = definition;
            Type = type;
            Method = method;

            // Marker
            MethodMarker = new MethodMarker();
            
            // Factories
            CloneFactory = new MethodCloneFactory(this);
            NameAliasFactory = new NameAliasFactory();
            ImporterFactory = new ImporterFactory(this);

            // Extensions
            MethodMixin = new MethodMixinExtension(this);
            MethodScope = new MethodScopingExtension(this);
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