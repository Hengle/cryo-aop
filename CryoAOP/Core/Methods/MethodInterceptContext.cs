using CryoAOP.Core.Factories;
using Mono.Cecil;

namespace CryoAOP.Core.Methods
{
    internal class MethodInterceptContext
    {
        public readonly MethodDefinition Definition;
        public readonly TypeIntercept TypeIntercept;
        
        public readonly MethodCloneFactory CloneFactory;
        public readonly StringAliasFactory StringAliasFactory;
        public readonly AssemblyImporterFactory ImporterFactory;
        
        public readonly MethodIntercept MethodIntercept;
        public readonly MethodInterceptMarker MethodMarker;
        public readonly MethodInterceptMixinExtension MethodMixin;
        public readonly MethodInterceptScopingExtension MethodScope;

        public MethodInterceptContext(TypeIntercept typeIntercept, MethodIntercept methodIntercept, MethodDefinition definition)
        {
            // Intercepts/Definitions
            Definition = definition;
            TypeIntercept = typeIntercept;
            MethodIntercept = methodIntercept;

            // Marker
            MethodMarker = new MethodInterceptMarker();
            
            // Factories
            CloneFactory = new MethodCloneFactory(this);
            StringAliasFactory = new StringAliasFactory();
            ImporterFactory = new AssemblyImporterFactory(this);

            // Extensions
            MethodMixin = new MethodInterceptMixinExtension(this);
            MethodScope = new MethodInterceptScopingExtension(this);
        }

        public AssemblyIntercept AssemblyIntercept
        {
            get { return TypeIntercept.AssemblyIntercept; }
        }

        public AssemblyDefinition AssemblyDefinition
        {
            get { return AssemblyIntercept.Definition; }
        }

        public TypeDefinition TypeDefinition
        {
            get { return TypeIntercept.Definition; }
        }
    }
}