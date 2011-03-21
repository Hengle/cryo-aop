using System;
using System.IO;
using System.Linq;
using CryoAOP.Core.Attributes;
using CryoAOP.Core.Extensions;
using CryoAOP.Aspects;

namespace CryoAOP.Core
{
    public partial class Intercept
    {
        internal static Assembly Assembly;

        public static void LoadAssembly(string assemblyPath)
        {
            Assembly = new Assembly(assemblyPath);
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
                    var typeInspector = new Type(Assembly, type);
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

        public static void InterceptProperty(string fullTypeName, string propertyName, MethodInterceptionScopeType interceptionScope)
        {
            var typeInspector = Assembly.FindType(fullTypeName);
            typeInspector.FindProperty(propertyName).InterceptProperty(interceptionScope);
        }

        public static void InterceptAspects()
        {
            var attributeFinder = new AttributeFinder();
            var results = attributeFinder.FindAttributes<InterceptAttribute>();

            var groupedByAssembly = results.GroupBy(r => r.Type.Assembly);
            foreach (var group in groupedByAssembly)
            {
                var shadowAssembly = group.First().ShadowAssembly;
                LoadAssembly(shadowAssembly.OriginalAssemblyPath);
                foreach (var result in group)
                {
                    if (result.IsForType())
                    {
                        InterceptType(result.TypeName, result.Attribute.Scope);
                        Console.WriteLine(
                            "CryoAOP -> Intercepted {0}\\{1}\\*"
                                .FormatWith(
                                    Path.GetFileName(
                                    shadowAssembly.OriginalAssemblyPath), 
                                    result.TypeName)); 
                    }
                    if (result.IsForMethod())
                    {
                        InterceptMethod(result.TypeName, result.MethodName, result.Attribute.Scope);
                        Console.WriteLine(
                            "CryoAOP -> Intercepted {0}\\{1}\\{2}"
                                .FormatWith(
                                    Path.GetFileName(shadowAssembly.OriginalAssemblyPath), 
                                    result.TypeName, 
                                    result.MethodName)); 
                    }
                    if (result.IsForProperty())
                    {
                        InterceptProperty(result.TypeName, result.PropertyName, result.Attribute.Scope);
                        Console.WriteLine(
                            "CryoAOP -> Intercepted {0}\\{1}\\{2}"
                                .FormatWith(
                                    Path.GetFileName(shadowAssembly.OriginalAssemblyPath),
                                    result.TypeName,
                                    result.PropertyName));
                    }
                }
                SaveAssembly(shadowAssembly.OriginalAssemblyPath);
            }
        }
    }
}
