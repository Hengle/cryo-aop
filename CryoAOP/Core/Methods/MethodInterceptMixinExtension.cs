using System;
using System.Linq;
using CryoAOP.Aspects;
using CryoAOP.Core.Attributes;
using CryoAOP.Core.Extensions;
using Mono.Cecil.Cil;

namespace CryoAOP.Core.Methods
{
    internal class MethodInterceptMixinExtension : MethodInterceptExtension
    {
        public const string MethodMarker = "CryoAOP -> Mixin";

        private readonly AttributeFinder attributeFinder = new AttributeFinder();

        public MethodInterceptMixinExtension(MethodInterceptContext context) : base(context)
        {
        }

        public void InsertMethodMixins()
        {
            var methods =
                attributeFinder
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
                    !methodInfo.Attribute.IsForType(TypeIntercept.Definition.Name))
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