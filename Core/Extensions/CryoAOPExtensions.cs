namespace CryoAOP.Core.Extensions
{
    public static class CryoAOPExtensions
    {
        public static MethodInspector GetMethod(this string[] args)
        {
            var assemblyInspector = new AssemblyInspector(args[0].Trim());
            var typeInspector = assemblyInspector.FindType(args[1].Trim());
            var methodInspector = typeInspector.FindMethod(args[2].Trim());
            return methodInspector;
        }
    }
}