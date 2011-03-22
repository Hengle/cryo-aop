//CryoAOP. Aspect Oriented Framework for .NET.
//Copyright (C) 2011  Gavin van der Merwe (fir3pho3nixx@gmail.com)

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CryoAOP.Exec;

namespace CryoAOP.Core.Attributes
{
    internal class AttributeFinder
    {
        private readonly AssemblyLoader loader = new AssemblyLoader();

        public IEnumerable<AttributeResult<T>> FindAttributes<T>() where T : Attribute
        {
            var assemblies = loader.GetShadowAssemblies();
            var attributesFound = new List<AttributeResult<T>>();
            foreach (var assembly in assemblies)
            {
                try
                {
                    foreach (var type in assembly.ShadowAssembly.GetTypes())
                    {
                        FindPropertyAttributes(assembly, type, attributesFound);
                        FindMethodAttributes(assembly, type, attributesFound);
                        FindTypeAttributes(assembly, type, attributesFound);
                    }
                }
                catch (Exception err3)
                {
                    "CryoAOP -> Warning! First chance exception ocurred while searching for Mixin Methods. \r\n{0}"
                        .Warn(err3);
                }
            }
            return attributesFound;
        }

        private static void FindPropertyAttributes<T>(ShadowAssemblyType shadowAssembly, System.Type type, List<AttributeResult<T>> attributesFound) where T : Attribute
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
                                    attr.GetType().FullName == typeof(T).FullName)
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

        private static void FindMethodAttributes<T>(ShadowAssemblyType shadowAssembly, System.Type type, List<AttributeResult<T>> attributesFound) where T : Attribute
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
                                    attr.GetType().FullName == typeof(T).FullName)
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

        private static void FindTypeAttributes<T>(ShadowAssemblyType shadowAssembly, System.Type type, List<AttributeResult<T>> attributesFound) where T : Attribute
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