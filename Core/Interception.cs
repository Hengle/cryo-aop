namespace CryoAOP.Core
{
    public class Interception
    {
        public static void RegisterType(string assemblyPath, string fullTypeName, string outputAssembly = null)
        {
            var assemblyInspector = new AssemblyInspector(assemblyPath);
            var typeInspector = assemblyInspector.FindType(fullTypeName);
            typeInspector.InterceptAllMethods();
            assemblyInspector.Write(outputAssembly ?? assemblyPath);
        }
        
        public static void RegisterType(string assemblyPath, string fullTypeName, string methodName, string outputAssembly = null)
        {
            var assemblyInspector = new AssemblyInspector(assemblyPath);
            var typeInspector = assemblyInspector.FindType(fullTypeName);
            typeInspector.FindMethod(methodName).InterceptMethod();
            assemblyInspector.Write(outputAssembly ?? assemblyPath);
        }
    }
}
