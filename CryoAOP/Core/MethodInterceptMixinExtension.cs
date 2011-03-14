using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CryoAOP.Core.Attributes;
using CryoAOP.Core.Extensions;
using CryoAOP.Exec;
using Mono.Cecil.Cil;

namespace CryoAOP.Core
{
    internal class MethodMixinInfo
    {
        public MethodMixinInfo(MethodInfo method, MixinMethodAttribute attribute)
        {
            Method = method;
            Attribute = attribute;
        }

        public MethodInfo Method { get; private set; }
        public MixinMethodAttribute Attribute { get; private set; }
    }

    internal class MethodInterceptMixinExtension : MethodInterceptExtension
    {
        public const string MethodMarker = "CryoAOP -> Mixin";

        public MethodInterceptMixinExtension(MethodInterceptContext context) : base(context)
        {
        }

        public void InsertMethodMixins()
        {
            var methods = FindMethodAttributes();
            foreach (var methodInfo in methods)
            {
                var methodSearchString =
                    "{0}, {1}"
                        .FormatWith(
                            methodInfo.Method.Name,
                            methodInfo.Method.GetParameters().Select(
                                p => p.ParameterType.Name).JoinWith(", "));

                // Mixin: Check if the attribute is type specific 
                if (methodInfo.Attribute.IsTypeSpecific && !methodInfo.Attribute.IsForType(TypeIntercept.Definition.Name))
                    continue;

                // Mixin: Check if the method has already been mixed in 
                var info = methodInfo;
                var methodAlreadyMixedIn =
                    TypeIntercept
                        .Definition
                        .Methods
                        .Any(searchMethod =>
                             Context.MethodMarker.HasMarker(searchMethod, MethodMarker)
                             && searchMethod.Name == info.Method.Name
                             && searchMethod.IsStatic == info.Method.IsStatic
                             && searchMethod.IsVirtual == info.Method.IsVirtual
                             && searchMethod.IsPrivate == info.Method.IsPrivate
                             && searchMethod.IsPublic == info.Method.IsPublic);

                if (methodAlreadyMixedIn)
                    continue;

                // Mixin: Clone the method signature
                var mixinMethod = Context.ImporterFactory.Import(methodInfo.Method.DeclaringType, methodSearchString);
                var cloneOfMixinMethod = Context.CloneFactory.CloneIntoType(mixinMethod, TypeIntercept.Definition);

                // Mixin: Clone the variables
                foreach (var mixinVariable in mixinMethod.Resolve().Body.Variables)
                {
                    var cloneOfMixinVariable =
                        new VariableDefinition(
                            mixinVariable.Name,
                            Context.ImporterFactory.Import(mixinVariable.VariableType));

                    cloneOfMixinMethod.Body.Variables.Add(cloneOfMixinVariable);
                }

                // Mixin: Init locals?
                cloneOfMixinMethod.Body.InitLocals = mixinMethod.Resolve().Body.InitLocals;

                // Mixin: Insert Mixin Marker
                Context.MethodMarker.CreateMarker(cloneOfMixinMethod, MethodMarker);

                // Mixin: Get IL processor
                var il = cloneOfMixinMethod.Body.GetILProcessor();

                // Mixin: Check if instance
                if (!cloneOfMixinMethod.IsStatic)
                {
                    // Mixin: Find default constructor on mixin
                    var mixinConstructorRef =
                        mixinMethod
                            .DeclaringType
                            .Resolve()
                            .Methods
                            .Where(m => m.IsConstructor)
                            .Single();

                    // Mixin: Create object with default constructor
                    il.Append(
                        new[]
                            {
                                il.Create(OpCodes.Newobj, Context.ImporterFactory.Import(mixinConstructorRef))
                            });
                }

                // Mixin: Load the arguments 
                foreach (var parameter in cloneOfMixinMethod.Parameters.ToList())
                    il.Append(il.Create(OpCodes.Ldarg, parameter));

                // Mixin: Check for generic params 
                if (cloneOfMixinMethod.HasGenericParameters)
                    il.Append(il.Create(OpCodes.Call, mixinMethod.MakeGeneric(cloneOfMixinMethod.GenericParameters.ToArray())));
                else
                    il.Append(il.Create(OpCodes.Call, mixinMethod));

                // Mixin: End of method 
                il.Append(il.Create(OpCodes.Ret));

                // Mixin: Done!
                Console.WriteLine("CryoAOP -> Mixed '{0}' into {1}".FormatWith(mixinMethod, cloneOfMixinMethod));
            }
        }

        private static IEnumerable<MethodMixinInfo> FindMethodAttributes()
        {
            var assemblies = LoadAssemblyList();
            var attributesFound = new List<MethodMixinInfo>();
            foreach (var assembly in assemblies)
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
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
                                    var methodMixinAttributes =
                                        method
                                            .GetCustomAttributes(true)
                                            .Where(
                                                attr =>
                                                attr.GetType().FullName == typeof (MixinMethodAttribute).FullName)
                                            .ToList();

                                    if (methodMixinAttributes.Count > 0)
                                    {
                                        var attribute = methodMixinAttributes.Cast<MixinMethodAttribute>().First();
                                        var info = new MethodMixinInfo(method, attribute);
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
                        catch (Exception err1)
                        {
                            "CryoAOP -> Warning! First chance exception ocurred while searching for Mixin Methods. \r\n{0}"
                                .Warn(err1);
                        }
                    }
                }
                catch (Exception err2)
                {
                    "CryoAOP -> Warning! First chance exception ocurred while searching for Mixin Methods. \r\n{0}"
                        .Warn(err2);
                }
            }
            return attributesFound;
        }

        private static IEnumerable<Assembly> LoadAssemblyList()
        {
            return AssemblyLoader.GetShadowAssemblies();
        }
    }
}