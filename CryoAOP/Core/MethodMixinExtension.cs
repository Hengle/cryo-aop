using System;
using System.Linq;
using CryoAOP.Core.Attributes;
using CryoAOP.Core.Extensions;
using Mono.Cecil.Cil;

namespace CryoAOP.Core
{
    internal class MethodMixinExtension : MethodExtension
    {
        public const string MethodMarker = "CryoAOP -> Mixin";

        private readonly AttributeSearch attributeSearch = new AttributeSearch();

        public MethodMixinExtension(MethodContext context) : base(context)
        {
        }

        public void MixinMethods()
        {
            var methods =
                attributeSearch
                    .FindAttributes<MixinMethodAttribute>();

            foreach (var methodInfo in methods)
            {
                var methodSearchString =
                    "{0}, {1}"
                        .FormatWith(
                            methodInfo.Method.Name,
                            methodInfo.Method.GetParameters().Select(
                                p => p.ParameterType.Name).JoinWith(", "));

                // Mixin: Check if the attribute is type specific 
                if (methodInfo.Attribute.IsTypeSpecific &&
                    !methodInfo.Attribute.IsForType(Type.Definition.Name))
                    continue;

                // Mixin: Check if the method has already been mixed in 
                var info = methodInfo;
                var methodAlreadyMixedIn =
                    Type
                        .Definition
                        .Methods
                        .Any(searchMethod =>
                             Context.Marker.HasMarker(searchMethod, MethodMarker)
                             && searchMethod.Name == info.Method.Name
                             && searchMethod.IsStatic == info.Method.IsStatic
                             && searchMethod.IsVirtual == info.Method.IsVirtual
                             && searchMethod.IsPrivate == info.Method.IsPrivate
                             && searchMethod.IsPublic == info.Method.IsPublic);

                if (methodAlreadyMixedIn)
                    continue;

                // Mixin: Clone the method signature
                var mixinMethod = Context.Importer.Import(methodInfo.Method.DeclaringType, methodSearchString);
                var cloneOfMixinMethod = Context.Cloning.CloneIntoType(mixinMethod, Type.Definition);

                // Mixin: Init locals?
                cloneOfMixinMethod.Body.InitLocals = mixinMethod.Resolve().Body.InitLocals;

                // Mixin: Insert Mixin Marker
                Context.Marker.CreateMarker(cloneOfMixinMethod, MethodMarker);

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
                            .First();

                    // Mixin: Create object with default constructor
                    il.Append(
                        new[]
                            {
                                il.Create(OpCodes.Newobj, Context.Importer.Import(mixinConstructorRef))
                            });
                }

                // Mixin: Load the arguments 
                foreach (var parameter in cloneOfMixinMethod.Parameters.ToList())
                    il.Append(il.Create(OpCodes.Ldarg, parameter));

                // Mixin: Check for generic params 
                if (cloneOfMixinMethod.HasGenericParameters)
                    il.Append(
                        il.Create(
                            OpCodes.Call,
                            mixinMethod.MakeGeneric(
                                cloneOfMixinMethod
                                    .GenericParameters
                                    .ToArray())));

                else
                    il.Append(il.Create(OpCodes.Call, mixinMethod));

                // Mixin: End of method 
                il.Append(il.Create(OpCodes.Ret));

                // Mixin: Done!
                Console.WriteLine("CryoAOP -> Mixed '{0}' into {1}".FormatWith(mixinMethod, cloneOfMixinMethod));
            }
        }
    }
}