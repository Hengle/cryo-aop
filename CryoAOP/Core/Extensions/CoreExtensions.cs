using CryoAOP.Core.Factories;

namespace CryoAOP.Core.Extensions
{
    internal static class CoreExtensions
    {
        public static Method GetMethod(this string[] args)
        {
            var assemblyFactory = new AssemblyFactory();
            var assemblyInspector = assemblyFactory.CreateAssembly(args[0].Trim());
            var typeInspector = assemblyInspector.FindType(args[1].Trim());
            var methodInspector = typeInspector.FindMethod(args[2].Trim());
            return methodInspector;
        }
    }
}