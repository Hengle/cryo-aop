using CryoAOP.Core.Factories;

namespace CryoAOP.Core
{
    internal class MethodInterceptExtension
    {
        protected readonly MethodIntercept MethodIntercept;
        protected readonly MethodCloneFactory CloneFactory;
        protected readonly AssemblyImporterFactory ImporterFactory;

        public MethodInterceptExtension(MethodIntercept methodIntercept)
        {
            this.MethodIntercept = methodIntercept;
            this.ImporterFactory = new AssemblyImporterFactory(methodIntercept);
            this.CloneFactory = new MethodCloneFactory();
        }

        protected TypeIntercept TypeIntercept
        {
            get { return MethodIntercept.TypeIntercept; }
        }
    }
}