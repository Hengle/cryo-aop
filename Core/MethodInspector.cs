using System;
using System.Linq;
using System.Reflection;
using CryoAOP.Core.Extensions;
using CryoAOP.Core.Factories;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CryoAOP.Core
{
    public class MethodInspector
    {
        public readonly MethodDefinition Definition;
        public readonly TypeInspector TypeInspector;
        private readonly MethodCloneFactory cloneFactory;
        private readonly AssemblyImporterFactory importerFactory;
        private readonly StringAliasFactory stringAliasFactory;

        public MethodInspector(TypeInspector typeInspector, MethodDefinition definition)
        {
            Definition = definition;
            TypeInspector = typeInspector;
            cloneFactory = new MethodCloneFactory();
            stringAliasFactory = new StringAliasFactory();
            importerFactory = new AssemblyImporterFactory(TypeInspector.AssemblyInspector.Definition);
        }

        public void Write(string assemblyPath)
        {
            TypeInspector.AssemblyInspector.Write(assemblyPath);
        }

        public void InterceptMethod()
        {
            var renamedMethod = Definition;
            var interceptorMethod = cloneFactory.Clone(Definition);
            renamedMethod.Name = stringAliasFactory.GenerateIdentityName(Definition.Name); ;

            // Insert interceptor code

            // Interceptor: Insert variables 
            var v_0 = new VariableDefinition("V_0", importerFactory.Import(typeof (Type)));
            interceptorMethod.Body.Variables.Add(v_0);
            var v_1 = new VariableDefinition("V_1", importerFactory.Import(typeof (MethodInfo)));
            interceptorMethod.Body.Variables.Add(v_1);
            var v_2 = new VariableDefinition("V_2", importerFactory.Import(typeof (MethodInvocation)));
            interceptorMethod.Body.Variables.Add(v_2);
            var v_3 = new VariableDefinition("V_3", importerFactory.Import(typeof (Object[])));
            interceptorMethod.Body.Variables.Add(v_3);
            var v_4 = new VariableDefinition("V_4", importerFactory.Import(typeof (Boolean)));
            interceptorMethod.Body.Variables.Add(v_4);

            // Interceptor: If has return type add to local variables
            if (renamedMethod.ReturnType.Name != "Void")
            {
                interceptorMethod.ReturnType = renamedMethod.ReturnType;
                var v_5 = new VariableDefinition("V_5", interceptorMethod.ReturnType);
                interceptorMethod.Body.Variables.Add(v_5);
            }

            // Interceptor: Init locals?
            interceptorMethod.Body.InitLocals = renamedMethod.Body.InitLocals;

            // Interceptor: Method return instruction 
            var endOfMethodInstruction = interceptorMethod.Body.GetILProcessor().Create(OpCodes.Nop);

            // Interceptor: Get IL Processor
            var il = interceptorMethod.Body.GetILProcessor();

            // Interceptor: Resolve type from handle uses V_0
            il.Append(new[]
                          {
                              il.Create(OpCodes.Nop),
                              il.Create(OpCodes.Ldtoken, TypeInspector.Definition),
                              il.Create(OpCodes.Call, importerFactory.Import(typeof (Type), "GetTypeFromHandle")),
                              il.Create(OpCodes.Stloc_0)
                          });

            // Interceptor: Get the method info 
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldloc_0),
                              il.Create(OpCodes.Ldstr, interceptorMethod.Name),
                              il.Create(OpCodes.Callvirt, importerFactory.Import(typeof (Type), "GetMethod,String")),
                              il.Create(OpCodes.Stloc_1)
                          });


            // Interceptor: Initialise object array for param values 
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldc_I4, interceptorMethod.Parameters.Count),
                              il.Create(OpCodes.Newarr, importerFactory.Import(typeof (Object))),
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
                                  il.Create(OpCodes.Ldarg, argIndex + 1),
                              });

                // Interceptor: Box up value types
                if (parameter.ParameterType.IsValueType)
                    il.Append(il.Create(OpCodes.Box, parameter.ParameterType));

                // Intreceptor: Allocate to array
                il.Append(il.Create(OpCodes.Stelem_Ref));
            }

            // Inteceptor: Initialise Method Invocation
            var methodInvocationTypRef = importerFactory.Import(typeof (MethodInvocation));
            var methodInvocationConstructor = methodInvocationTypRef.Resolve().Methods.Where(m => m.IsConstructor).First();

            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldloc_0),
                              il.Create(OpCodes.Ldloc_1),
                              il.Create(OpCodes.Ldloc_3),
                              il.Create(OpCodes.Newobj, importerFactory.Import(methodInvocationConstructor)),
                              il.Create(OpCodes.Stloc_2)
                          });

            // Interceptor: Call interceptor method 
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldloc_2),
                              il.Create(OpCodes.Call, importerFactory.Import(typeof (GlobalInterceptor), "HandleInvocation"))
                          });


            // Interceptor: If not void push result from interception
            if (renamedMethod.ReturnType.Name != "Void")
            {
                il.Append(new[]
                              {
                                  il.Create(OpCodes.Ldloc_2),
                                  il.Create(OpCodes.Callvirt, importerFactory.Import(typeof (MethodInvocation), "get_Result")),
                                  il.Create(OpCodes.Stloc, 5)
                              });
            }

            // Interceptor: Check if invocation has been cancelled
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldloc_2),
                              il.Create(OpCodes.Callvirt, importerFactory.Import(typeof (MethodInvocation), "get_CanInvoke")),
                              il.Create(OpCodes.Ldc_I4_0),
                              il.Create(OpCodes.Ceq),
                              il.Create(OpCodes.Stloc, 4),
                              il.Create(OpCodes.Ldloc, 4),
                              il.Create(OpCodes.Brtrue, endOfMethodInstruction)
                          });

            // Interceptor: Insert IL call from clone to renamed method
            il.Append(il.Create(OpCodes.Ldarg_0));

            foreach (var parameter in interceptorMethod.Parameters)
            {
                var argIndex = interceptorMethod.Parameters.IndexOf(parameter);
                il.Append(il.Create(OpCodes.Ldarg, (ushort) (argIndex + 1)));
            }

            if (renamedMethod.HasGenericParameters)
                il.Append(il.Create(OpCodes.Call, renamedMethod.MakeGeneric(interceptorMethod.GenericParameters.ToArray())));
            else
                il.Append(il.Create(OpCodes.Call, renamedMethod));

            // Interceptor: Store method return value);)
            if (interceptorMethod.ReturnType.Name != "Void")
            {
                il.Append(il.Create(OpCodes.Stloc, 5));
            }

            il.Append(il.Create(OpCodes.Nop));


            // Interceptor: Set return type on MethodInvocation )
            if (interceptorMethod.ReturnType.Name != "Void")
            {
                if (renamedMethod.ReturnType.IsValueType)
                {
                    // ! Experimental ! - Works!
                    il.Append(new[]
                                  {
                                      il.Create(OpCodes.Ldloc_2),
                                      il.Create(OpCodes.Ldloc, 5),
                                      il.Create(OpCodes.Box, renamedMethod.ReturnType),
                                      il.Create(OpCodes.Callvirt, importerFactory.Import(typeof (MethodInvocation), "set_Result"))
                                  });
                }
                else
                {
                    il.Append(new[]
                                  {
                                      il.Create(OpCodes.Ldloc_2),
                                      il.Create(OpCodes.Ldloc, 5),
                                      il.Create(OpCodes.Callvirt, importerFactory.Import(typeof (MethodInvocation), "set_Result"))
                                  });
                }
            }

            // Interceptor: Continue the invocation by changing state
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldloc_2),
                              il.Create(OpCodes.Call, importerFactory.Import(typeof (MethodInvocation), "ContinueInvocation"))
                          });

            // Interceptor: Do post invocation call 
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldloc_2),
                              il.Create(OpCodes.Call, importerFactory.Import(typeof (GlobalInterceptor), "HandleInvocation"))
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
                                      il.Create(OpCodes.Callvirt, importerFactory.Import(typeof (MethodInvocation), "get_Result")),
                                      il.Create(OpCodes.Unbox_Any, interceptorMethod.ReturnType),
                                      il.Create(OpCodes.Stloc, 5),
                                      il.Create(OpCodes.Ldloc, 5)
                                  });
                }
                else
                {
                    il.Append(new[]
                                  {
                                      il.Create(OpCodes.Ldloc_2),
                                      il.Create(OpCodes.Callvirt, importerFactory.Import(typeof (MethodInvocation), "get_Result")),
                                  });
                }

            }

            il.Append(il.Create(OpCodes.Ret));
        }

        public override string ToString()
        {
            return "{0}".FormatWith(Definition.FullName);
        }
    }
}