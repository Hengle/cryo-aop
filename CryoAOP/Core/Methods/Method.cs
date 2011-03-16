using System;
using System.Linq;
using System.Reflection;
using CryoAOP.Aspects;
using CryoAOP.Core.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CryoAOP.Core.Methods
{
    internal class Method
    {
        public const string MethodMarker = "CryoAOP -> Intercept";
        protected readonly MethodContext Context;

        public MethodDefinition Definition
        {
            get { return Context.Definition; }
        }

        public Type Type
        {
            get { return Context.Type; }
        }

        public Method(Type type, MethodDefinition definition)
        {
            Context = new MethodContext(type, this, definition);
        }

        public void Write(string assemblyPath)
        {
            Context
                .Assembly
                .Write(assemblyPath);
        }

        public void InterceptMethod(MethodInterceptionScopeType interceptionScope = MethodInterceptionScopeType.Shallow)
        {
            var renamedMethod = Context.Definition;

            var interceptorMethod =
                Context
                    .CloneFactory
                    .Clone(Context.Definition);

            renamedMethod.Name =
                Context
                    .NameAliasFactory
                    .GenerateIdentityName(Context.Definition.Name);

            // Insert interceptor code

            // Interceptor: Insert variables 
            var v0 = new VariableDefinition("V_0", Context.ImporterFactory.Import(typeof (System.Type)));
            interceptorMethod.Body.Variables.Add(v0);
            var v1 = new VariableDefinition("V_1", Context.ImporterFactory.Import(typeof (MethodInfo)));
            interceptorMethod.Body.Variables.Add(v1);
            var v2 = new VariableDefinition("V_2", Context.ImporterFactory.Import(typeof (MethodInvocation)));
            interceptorMethod.Body.Variables.Add(v2);
            var v3 = new VariableDefinition("V_3", Context.ImporterFactory.Import(typeof (Object[])));
            interceptorMethod.Body.Variables.Add(v3);
            var v4 = new VariableDefinition("V_4", Context.ImporterFactory.Import(typeof (Boolean)));
            interceptorMethod.Body.Variables.Add(v4);

            // Interceptor: If has return type add to local variables
            if (renamedMethod.ReturnType.Name != "Void")
            {
                interceptorMethod.ReturnType = renamedMethod.ReturnType;
                var v5 = new VariableDefinition("V_5", interceptorMethod.ReturnType);
                interceptorMethod.Body.Variables.Add(v5);
            }

            // Interceptor: Init locals?
            interceptorMethod.Body.InitLocals = renamedMethod.Body.InitLocals;

            // Interceptor: Method return instruction 
            var endOfMethodInstruction = interceptorMethod.Body.GetILProcessor().Create(OpCodes.Nop);

            // Interceptor: Get IL Processor
            var il = interceptorMethod.Body.GetILProcessor();

            // Interceptor: Insert interceptor marker
            Context.MethodMarker.CreateMarker(interceptorMethod, MethodMarker);

            // Interceptor: Resolve type from handle uses V_0
            il.Append(new[]
                          {
                              il.Create(OpCodes.Nop),
                              il.Create(OpCodes.Ldtoken, Context.Type.Definition),
                              il.Create(OpCodes.Call, Context.ImporterFactory.Import(typeof (System.Type), "GetTypeFromHandle")),
                              il.Create(OpCodes.Stloc_0)
                          });

            // Interceptor: Get the method info 
            var methodReference = Context.ImporterFactory.Import(typeof (System.Type), "GetMethod, String, BindingFlags");
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldloc_0),
                              il.Create(OpCodes.Ldstr, interceptorMethod.Name),
                              il.Create(OpCodes.Ldc_I4_S, (sbyte) 60),
                              // BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance
                              il.Create(OpCodes.Callvirt, methodReference),
                              il.Create(OpCodes.Stloc_1)
                          });


            // Interceptor: Initialise object array for param values 
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldc_I4, interceptorMethod.Parameters.Count),
                              il.Create(OpCodes.Newarr, Context.ImporterFactory.Import(typeof (Object))),
                              il.Create(OpCodes.Stloc_3)
                          });

            foreach (var parameter in interceptorMethod.Parameters)
            {
                var argIndex = interceptorMethod.Parameters.IndexOf(parameter);

                // Interceptor: Load the argument for array
                il.Append(new[]
                              {
                                  il.Create(OpCodes.Ldloc_3),
                                  il.Create(OpCodes.Ldc_I4, argIndex),
                                  il.Create(OpCodes.Ldarg, parameter),
                              });

                // Interceptor: Box up value types
                if (parameter.ParameterType.IsValueType || parameter.ParameterType.IsGenericParameter)
                    il.Append(il.Create(OpCodes.Box, parameter.ParameterType));

                // Intreceptor: Allocate to array
                il.Append(il.Create(OpCodes.Stelem_Ref));
            }

            // Inteceptor: Initialise Method Invocation
            var methodInvocationTypRef = Context.ImporterFactory.Import(typeof (MethodInvocation));

            var methodInvocationConstructors =
                methodInvocationTypRef
                    .Resolve()
                    .Methods
                    .Where(m => m.IsConstructor)
                    .ToList();

            // Interceptor: Get instance or static based constructor
            MethodDefinition methodInvocationConstructor;
            if (renamedMethod.IsStatic)
            {
                // If static
                methodInvocationConstructor =
                    methodInvocationConstructors
                        .Where(c => c.Parameters[0].ParameterType.Name.ToLower().IndexOf("type") != -1)
                        .First();
            }
            else
            {
                // If instance
                methodInvocationConstructor =
                    methodInvocationConstructors
                        .Where(c => c.Parameters[0].ParameterType.Name.ToLower().IndexOf("object") != -1)
                        .First();

                // Load 'this'
                il.Append(il.Create(OpCodes.Ldarg_0));
            }

            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldloc_0),
                              il.Create(OpCodes.Ldloc_1),
                              il.Create(OpCodes.Ldloc_3),
                              il.Create(OpCodes.Newobj, Context.ImporterFactory.Import(methodInvocationConstructor)),
                              il.Create(OpCodes.Stloc_2)
                          });

            // Interceptor: Call interceptor method 
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldloc_2),
                              il.Create(OpCodes.Call, Context.ImporterFactory.Import(typeof (Intercept), "HandleInvocation"))
                          });


            // Interceptor: If not void push result from interception
            if (renamedMethod.ReturnType.Name != "Void")
            {
                il.Append(new[]
                              {
                                  il.Create(OpCodes.Ldloc_2),
                                  il.Create(OpCodes.Callvirt,
                                            Context.ImporterFactory.Import(typeof (MethodInvocation), "get_Result")),
                                  il.Create(OpCodes.Stloc, 5)
                              });
            }

            // Interceptor: Check if invocation has been cancelled
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldloc_2),
                              il.Create(OpCodes.Callvirt,
                                        Context.ImporterFactory.Import(typeof (MethodInvocation), "get_CanInvoke")),
                              il.Create(OpCodes.Ldc_I4_0),
                              il.Create(OpCodes.Ceq),
                              il.Create(OpCodes.Stloc, 4),
                              il.Create(OpCodes.Ldloc, 4),
                              il.Create(OpCodes.Brtrue, endOfMethodInstruction)
                          });

            // Interceptor: Insert IL call from clone to renamed method
            if (!renamedMethod.IsStatic)
                il.Append(il.Create(OpCodes.Ldarg_0));

            //  Interceptor: Load args for method call
            foreach (var parameter in interceptorMethod.Parameters.ToList())
                il.Append(il.Create(OpCodes.Ldarg, parameter));

            if (renamedMethod.HasGenericParameters)
                il.Append(il.Create(OpCodes.Call, renamedMethod.MakeGeneric(interceptorMethod.GenericParameters.ToArray())));
            else
                il.Append(il.Create(OpCodes.Call, renamedMethod));

            // Interceptor: Store method return value
            if (interceptorMethod.ReturnType.Name != "Void")
            {
                il.Append(il.Create(OpCodes.Stloc, 5));
            }

            // Interceptor: Set return type on MethodInvocation 
            if (interceptorMethod.ReturnType.Name != "Void")
            {
                if (renamedMethod.ReturnType.IsValueType)
                {
                    il.Append(new[]
                                  {
                                      il.Create(OpCodes.Ldloc_2),
                                      il.Create(OpCodes.Ldloc, 5),
                                      il.Create(OpCodes.Box, renamedMethod.ReturnType),
                                      il.Create(OpCodes.Callvirt,
                                                Context.ImporterFactory.Import(typeof (MethodInvocation), "set_Result"))
                                  });
                }
                else
                {
                    il.Append(new[]
                                  {
                                      il.Create(OpCodes.Ldloc_2),
                                      il.Create(OpCodes.Ldloc, 5),
                                      il.Create(OpCodes.Callvirt,
                                                Context.ImporterFactory.Import(typeof (MethodInvocation), "set_Result"))
                                  });
                }
            }

            // Interceptor: Continue the invocation by changing state
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldloc_2),
                              il.Create(OpCodes.Call,
                                        Context.ImporterFactory.Import(typeof (MethodInvocation), "ContinueInvocation"))
                          });

            // Interceptor: Do post invocation call 
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldloc_2),
                              il.Create(OpCodes.Call, Context.ImporterFactory.Import(typeof (Intercept), "HandleInvocation"))
                          });

            // Interceptor: End of method, doing this in advance for branching ?CancelInvocation?.
            il.Append(endOfMethodInstruction);


            // Interceptor: Loading the result from the invocation 
            if (interceptorMethod.ReturnType.Name != "Void")
            {
                if (renamedMethod.ReturnType.IsValueType)
                {
                    il.Append(new[]
                                  {
                                      il.Create(OpCodes.Ldloc_2),
                                      il.Create(OpCodes.Callvirt,
                                                Context.ImporterFactory.Import(typeof (MethodInvocation), "get_Result")),
                                      il.Create(OpCodes.Unbox_Any, interceptorMethod.ReturnType),
                                  });
                }
                else
                {
                    il.Append(new[]
                                  {
                                      il.Create(OpCodes.Ldloc_2),
                                      il.Create(OpCodes.Callvirt,
                                                Context.ImporterFactory.Import(typeof (MethodInvocation), "get_Result")),
                                  });
                }
            }

            // Interceptor: Return
            il.Append(il.Create(OpCodes.Ret));

            // If deep intercept, replace internals with call to renamed method
            Context.MethodScope.ApplyDeepScope(renamedMethod, interceptorMethod, il, interceptionScope);

            // Insert code mixins into intercepted types
            Context.MethodMixin.InsertMethodMixins();
        }

        public override string ToString()
        {
            return "{0}".FormatWith(Context.Definition.FullName);
        }
    }

    internal static class Mixin
    {
    }
}