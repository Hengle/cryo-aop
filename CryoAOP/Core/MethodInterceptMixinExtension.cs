using System;
using System.Diagnostics;
using System.Linq;
using CryoAOP.Core.Attributes;
using CryoAOP.Core.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CryoAOP.Core
{
    internal class MethodInterceptMixinExtension : MethodInterceptExtension
    {
        private readonly AttributeFinder attributeFinder = new AttributeFinder();

        public MethodInterceptMixinExtension(MethodIntercept methodIntercept) : base(methodIntercept)
        {
        }

        public void InsertCodeMixins()
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
                             HasMixinMarker(searchMethod)
                             && searchMethod.Name == info.Method.Name
                             && searchMethod.IsStatic == info.Method.IsStatic
                             && searchMethod.IsVirtual == info.Method.IsVirtual
                             && searchMethod.IsPrivate == info.Method.IsPrivate
                             && searchMethod.IsPublic == info.Method.IsPublic);

                if (methodAlreadyMixedIn)
                    continue;

                // Mixin: Clone the method signature
                var mixinMethod = ImporterFactory.Import(methodInfo.Method.DeclaringType, methodSearchString);
                var cloneOfMixinMethod = CloneFactory.CloneIntoType(mixinMethod, TypeIntercept.Definition);

                // Mixin: Init locals?
                cloneOfMixinMethod.Body.InitLocals = mixinMethod.Resolve().Body.InitLocals;

                // Mixin: Insert Mixin Marker
                CreateMixinMarker(cloneOfMixinMethod);

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
                                il.Create(OpCodes.Newobj, ImporterFactory.Import(mixinConstructorRef))
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

        public virtual void CreateMixinMarker(MethodDefinition method)
        {
            var il = method.Body.GetILProcessor();
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldstr, "CryoAOP -> Mixin"),
                              il.Create(OpCodes.Pop)
                          });
        }

        public virtual bool HasMixinMarker(MethodDefinition method)
        {
            if (method == null
                || method.Body == null
                || method.Body.Instructions == null
                || method.Body.Instructions.Count <= 2)
                return false;

            var interceptMarker = method.Body.Instructions.ToList().Take(2).ToArray();
            var firstInstruction = interceptMarker.First();
            return
                firstInstruction.OpCode == OpCodes.Ldstr
                && (firstInstruction.Operand as string) == "CryoAOP -> Mixin";
        }
    }
}