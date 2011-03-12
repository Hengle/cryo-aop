namespace CryoAOP.Core
{
    public partial class Intercept
    {
        internal static AssemblyIntercept Assembly;

        public static void LoadAssembly(string assemblyPath)
        {
            Assembly = new AssemblyIntercept(assemblyPath);
        }

        public static void SaveAssembly(string assemblyPath)
        {
            Assembly.Write(assemblyPath);
        }

        public static void InterceptAll(MethodInterceptionScopeType interceptionScope)
        {
            foreach (var module in Assembly.Definition.Modules)
            {
                foreach (var type in module.Types)
                {
                    var typeInspector = new TypeIntercept(Assembly, type);
                    typeInspector.InterceptAll(interceptionScope);
                }
            }
        }

        public static void InterceptType(string fullTypeName, MethodInterceptionScopeType interceptionScope)
        {
            var typeInspector = Assembly.FindType(fullTypeName);
            typeInspector.InterceptAll(interceptionScope);
        }

        public static void InterceptMethod(string fullTypeName, string methodName, MethodInterceptionScopeType interceptionScope)
        {
            var typeInspector = Assembly.FindType(fullTypeName);
            typeInspector.FindMethod(methodName).InterceptMethod(interceptionScope);
        }
    }
}
