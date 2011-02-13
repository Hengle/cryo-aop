using System;
using System.Linq;
using System.Reflection;
using CryoAOP.Core.Exceptions;

namespace CryoAOP.Core.Extensions
{
    public static class ReflectionExtensions
    {
        public static Type FindType(this Assembly assembly, string typeName)
        {
            var type = assembly.GetTypes().Where(t => t.FullName.ToLower().EndsWith(typeName.ToLower())).FirstOrDefault();
            if (type == null)
                throw new TypeNotFoundException("Could not find type '{0}' in '{1}'", typeName, assembly.FullName);
            return type;
        }

        public static object Invoke(this MethodInfo method, params object[] args)
        {
            var type = method.DeclaringType;
            var instance = type.Assembly.CreateInstance(type.FullName);
            var returnValue = method.Invoke(instance, args);
            return returnValue;
        }
    }
}