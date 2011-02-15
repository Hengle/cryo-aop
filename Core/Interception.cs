namespace CryoAOP.Core
{
    public class Interception
    {
        public static void RegisterType(string assemblyPath, string fullTypeName, string outputAssemblyPath = null)
        {
            var assemblyInspector = new AssemblyInspector(assemblyPath);
            TypeInspector typeInspector = assemblyInspector.FindType(fullTypeName);
            typeInspector.InterceptAllMethods();
            assemblyInspector.Write(outputAssemblyPath ?? assemblyPath);
        }
    }
}