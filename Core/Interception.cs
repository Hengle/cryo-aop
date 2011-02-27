namespace CryoAOP.Core
{
    public class Interception
    {
        public static void RegisterType(string assemblyPath, string fullTypeName, string outputAssembly = null)
        {
            RegisterType(assemblyPath, fullTypeName, MethodInterceptionScope.Shallow, outputAssembly);
        }

        public static void RegisterType(string assemblyPath, string fullTypeName, MethodInterceptionScope interceptionScope, string outputAssembly = null)
        {
            var assemblyInspector = new AssemblyInspector(assemblyPath);
            var typeInspector = assemblyInspector.FindType(fullTypeName);
            typeInspector.InterceptAllMethods(interceptionScope);
            assemblyInspector.Write(outputAssembly ?? assemblyPath);
        }

        public static void RegisterType(string assemblyPath, string fullTypeName, string methodName, string outputAssembly = null)
        {
            RegisterType(assemblyPath, fullTypeName, methodName, MethodInterceptionScope.Shallow, outputAssembly);
        }

        public static void RegisterType(string assemblyPath, string fullTypeName, string methodName, MethodInterceptionScope interceptionScope, string outputAssembly = null)
        {
            var assemblyInspector = new AssemblyInspector(assemblyPath);
            var typeInspector = assemblyInspector.FindType(fullTypeName);
            typeInspector.FindMethod(methodName).InterceptMethod(interceptionScope);
            assemblyInspector.Write(outputAssembly ?? assemblyPath);
        }
    }
}
