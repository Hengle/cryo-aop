using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CryoAOP.Core.Cache;
using CryoAOP.Exec;

namespace CryoAOP.Core.Attributes
{
    internal class AttributeSearch
    {
        private static readonly IMemoryCacheGeneric AttributeCache = new MemoryCacheGeneric();
        private readonly AssemblyLoader loader = new AssemblyLoader();

        public IEnumerable<AttributeResult<T>> FindAttributes<T>() where T : Attribute
        {
            var assemblies = loader.GetShadowAssemblies();

            var attributesFound = new List<AttributeResult<T>>();
            foreach (var assembly in assemblies)
            {
                var shadowAssemblyHash = assembly.ShadowAssembly.FullName;
                if (AttributeCache.ContainsKey<AttributeCache<T>>(shadowAssemblyHash))
                    attributesFound.AddRange(AttributeCache.Get<AttributeCache<T>>(shadowAssemblyHash).Attributes);
                else
                {
                    try
                    {
                        foreach (var type in assembly.ShadowAssembly.GetTypes())
                        {
                            FindPropertyAttributes(assembly, type, attributesFound);
                            FindMethodAttributes(assembly, type, attributesFound);
                            FindTypeAttributes(assembly, type, attributesFound);
                        }
                        AttributeCache.Set(shadowAssemblyHash, new AttributeCache<T>(assembly, attributesFound));
                    }
                    catch (Exception err3)
                    {
                        "CryoAOP -> Warning! First chance exception ocurred while searching for Mixin Methods. \r\n{0}"
                            .Warn(err3);
                    }
                }
            }
            return attributesFound;
        }

        private static void FindPropertyAttributes<T>(ShadowAssemblyType shadowAssembly, System.Type type,
                                                      List<AttributeResult<T>> attributesFound) where T : Attribute
        {
            try
            {
                var properties =
                    type.GetProperties(
                        BindingFlags.Public
                        | BindingFlags.NonPublic
                        | BindingFlags.Static
                        | BindingFlags.Instance);

                foreach (var property in properties)
                {
                    try
                    {
                        var propertyAttributes =
                            property
                                .GetCustomAttributes(true)
                                .Where(
                                    attr =>
                                    attr.GetType().FullName == typeof (T).FullName)
                                .ToList();

                        if (propertyAttributes.Count > 0)
                        {
                            var attribute = propertyAttributes.Cast<T>().First();
                            var info = new AttributeResult<T>(shadowAssembly, type, property, attribute);
                            attributesFound.Add(info);
                        }
                    }
                    catch (Exception err)
                    {
                        "CryoAOP -> Warning! First chance exception ocurred while searching for Mixin Methods. \r\n{0}"
                            .Warn(err);
                    }
                }
            }
            catch (Exception err2)
            {
                "CryoAOP -> Warning! First chance exception ocurred while searching for Mixin Methods. \r\n{0}"
                    .Warn(err2);
            }
        }

        private static void FindMethodAttributes<T>(ShadowAssemblyType shadowAssembly, System.Type type,
                                                    List<AttributeResult<T>> attributesFound) where T : Attribute
        {
            try
            {
                var methods =
                    type.GetMethods(
                        BindingFlags.Public
                        | BindingFlags.NonPublic
                        | BindingFlags.Static
                        | BindingFlags.Instance);

                foreach (var method in methods)
                {
                    try
                    {
                        var methodAttributes =
                            method
                                .GetCustomAttributes(true)
                                .Where(
                                    attr =>
                                    attr.GetType().FullName == typeof (T).FullName)
                                .ToList();

                        if (methodAttributes.Count > 0)
                        {
                            var attribute = methodAttributes.Cast<T>().First();
                            var info = new AttributeResult<T>(shadowAssembly, type, method, attribute);
                            attributesFound.Add(info);
                        }
                    }
                    catch (Exception err)
                    {
                        "CryoAOP -> Warning! First chance exception ocurred while searching for Mixin Methods. \r\n{0}"
                            .Warn(err);
                    }
                }
            }
            catch (Exception err2)
            {
                "CryoAOP -> Warning! First chance exception ocurred while searching for Mixin Methods. \r\n{0}"
                    .Warn(err2);
            }
        }

        private static void FindTypeAttributes<T>(ShadowAssemblyType shadowAssembly, System.Type type,
                                                  List<AttributeResult<T>> attributesFound) where T : Attribute
        {
            try
            {
                var typeAttributes =
                    type
                        .GetCustomAttributes(true)
                        .Where(
                            attr =>
                            attr.GetType().FullName == typeof (T).FullName)
                        .ToList();

                if (typeAttributes.Count > 0)
                {
                    var attribute = typeAttributes.Cast<T>().First();
                    var info = new AttributeResult<T>(shadowAssembly, type, attribute);
                    attributesFound.Add(info);
                }
            }
            catch (Exception err1)
            {
                "CryoAOP -> Warning! First chance exception ocurred while searching for Mixin Methods. \r\n{0}"
                    .Warn(err1);
            }
        }
    }
}