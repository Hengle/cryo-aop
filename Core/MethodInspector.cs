using System;
using System.Linq;
using System.Reflection;
using CryoAOP.Core.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CryoAOP.Core
{
    public class MethodInspector
    {
        public readonly MethodDefinition Definition;
        public readonly TypeInspector TypeInspector;

        public MethodInspector(TypeInspector typeInspector, MethodDefinition definition)
        {
            TypeInspector = typeInspector;
            Definition = definition;
        }

        public void Write(string assemblyPath)
        {
            TypeInspector.AssemblyInspector.Write(assemblyPath);
        }

        public void InterceptMethod(string methodPrefix = null)
        {
            // Generate prefix
            if (methodPrefix == null)
                methodPrefix = "_{0}_".FormatWith(Guid.NewGuid().ToString("N"));

            // Create new Method 
            var interceptorMethod = new MethodDefinition(Definition.Name, Definition.Attributes, Definition.ReturnType);
            Definition.DeclaringType.Methods.Add(interceptorMethod);

            // Rename existing method 
            var renamedMethod = Definition;
            renamedMethod.Name = "{0}{1}".FormatWith(methodPrefix, renamedMethod.Name);

            // Copy attributes
            CloneMethodProperties(interceptorMethod, renamedMethod);

            // Copy calling convention
            interceptorMethod.CallingConvention = renamedMethod.CallingConvention;

            // Copy method semantics 
            interceptorMethod.SemanticsAttributes = renamedMethod.SemanticsAttributes;

            // Copy attributes 
            renamedMethod.CustomAttributes.ToList().ForEach(a => interceptorMethod.CustomAttributes.Add(a));

            // Copy security declarations 
            renamedMethod.SecurityDeclarations.ToList().ForEach(s => interceptorMethod.SecurityDeclarations.Add(s));

            // Copy pinvoke info, dont do this! Sets method body to null!
            //interceptorMethod.PInvokeInfo = renamedMethod.PInvokeInfo;

            // Copy parameters across
            if (renamedMethod.HasParameters)
                foreach (var parameter in renamedMethod.Parameters.ToList())
                    interceptorMethod.Parameters.Add(parameter);

            // Copy generic parameters across -> Move to clone factory
            if (renamedMethod.HasGenericParameters)
            {
                foreach (var genericParameter in renamedMethod.GenericParameters.ToList())
                {
                    if (genericParameter != null)
                    {
                        var newGenericParameter = new GenericParameter(genericParameter.Name, interceptorMethod);
                        interceptorMethod.GenericParameters.Add(newGenericParameter);
                        CloneGenericParameterProperties(genericParameter, newGenericParameter);
                        //Console.WriteLine(genericParameter.InstanceDiff(newGenericParameter));
                    }
                }
            }

            // Insert interceptor code

            // Interceptor: Insert variables 
            var v_0 = new VariableDefinition("V_0", TypeInspector.AssemblyInspector.Import(typeof (Type)));
            interceptorMethod.Body.Variables.Add(v_0);
            var v_1 = new VariableDefinition("V_1", TypeInspector.AssemblyInspector.Import(typeof (MethodInfo)));
            interceptorMethod.Body.Variables.Add(v_1);
            var v_2 = new VariableDefinition("V_2", TypeInspector.AssemblyInspector.Import(typeof (MethodInvocation)));
            interceptorMethod.Body.Variables.Add(v_2);
            var v_3 = new VariableDefinition("V_3", TypeInspector.AssemblyInspector.Import(typeof (Object[])));
            interceptorMethod.Body.Variables.Add(v_3);
            var v_4 = new VariableDefinition("V_4", TypeInspector.AssemblyInspector.Import(typeof (Boolean)));
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
                              il.Create(OpCodes.Call, Import(typeof (Type), "GetTypeFromHandle")),
                              il.Create(OpCodes.Stloc_0)
                          });

            // Interceptor: Get the method info 
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldloc_0),
                              il.Create(OpCodes.Ldstr, interceptorMethod.Name),
                              il.Create(OpCodes.Callvirt, Import(typeof (Type), "GetMethod,String")),
                              il.Create(OpCodes.Stloc_1)
                          });


            // Interceptor: Initialise object array for param values 
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldc_I4, interceptorMethod.Parameters.Count),
                              il.Create(OpCodes.Newarr, Import(typeof (Object))),
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
            var methodInvocationTypRef = TypeInspector.AssemblyInspector.Import(typeof (MethodInvocation));
            var methodInvocationConstructor = methodInvocationTypRef.Resolve().Methods.Where(m => m.IsConstructor).First();

            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldloc_0),
                              il.Create(OpCodes.Ldloc_1),
                              il.Create(OpCodes.Ldloc_3),
                              il.Create(OpCodes.Newobj, Import(methodInvocationConstructor)),
                              il.Create(OpCodes.Stloc_2)
                          });

            // Interceptor: Call interceptor method 
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldloc_2),
                              il.Create(OpCodes.Call, Import(typeof (GlobalInterceptor), "HandleInvocation"))
                          });


            // Interceptor: If not void push result from interception
            if (renamedMethod.ReturnType.Name != "Void")
            {
                il.Append(new[]
                              {
                                  il.Create(OpCodes.Ldloc_2),
                                  il.Create(OpCodes.Callvirt, Import(typeof (MethodInvocation), "get_Result")),
                                  il.Create(OpCodes.Stloc, 5)
                              });
            }

            // Interceptor: Check if invocation has been cancelled
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldloc_2),
                              il.Create(OpCodes.Callvirt, Import(typeof (MethodInvocation), "get_CanInvoke")),
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
                                      il.Create(OpCodes.Callvirt, Import(typeof (MethodInvocation), "set_Result"))
                                  });
                }
                else
                {
                    il.Append(new[]
                                  {
                                      il.Create(OpCodes.Ldloc_2),
                                      il.Create(OpCodes.Ldloc, 5),
                                      il.Create(OpCodes.Callvirt, Import(typeof (MethodInvocation), "set_Result"))
                                  });
                }
            }

            // Interceptor: Continue the invocation by changing state
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldloc_2),
                              il.Create(OpCodes.Call, Import(typeof (MethodInvocation), "ContinueInvocation"))
                          });

            // Interceptor: Do post invocation call 
            il.Append(new[]
                          {
                              il.Create(OpCodes.Ldloc_2),
                              il.Create(OpCodes.Call, Import(typeof (GlobalInterceptor), "HandleInvocation"))
                          });

            // Interceptor: End of method
            il.Append(endOfMethodInstruction);


            // Interceptor: Loading the result from the invocation 
            if (interceptorMethod.ReturnType.Name != "Void")
            {
                if (renamedMethod.ReturnType.IsValueType)
                {
                    il.Append(new[]
                                  {
                                      il.Create(OpCodes.Ldloc_2),
                                      il.Create(OpCodes.Callvirt, Import(typeof (MethodInvocation), "get_Result")),
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
                                      il.Create(OpCodes.Callvirt, Import(typeof (MethodInvocation), "get_Result")),
                                  });
                }

            }

            il.Append(il.Create(OpCodes.Ret));

            // TODO: Debug code, remove it!
            //Console.WriteLine(renamedMethod.InstanceDiff(interceptorMethod));
        }

        // TODO: Move to clone factory
        private static void CloneGenericParameterProperties(GenericParameter genericParameter, GenericParameter newGenericParameter)
        {
            newGenericParameter.Attributes = genericParameter.Attributes;
            genericParameter.Constraints.ForEach(gp => newGenericParameter.Constraints.Add(gp));
            genericParameter.CustomAttributes.ForEach(ca => newGenericParameter.CustomAttributes.Add(ca));
            newGenericParameter.DeclaringType = genericParameter.DeclaringType;
            genericParameter.GenericParameters.ForEach(gp => newGenericParameter.GenericParameters.Add(gp));
            newGenericParameter.HasDefaultConstructorConstraint = genericParameter.HasDefaultConstructorConstraint;
            newGenericParameter.IsContravariant = genericParameter.IsContravariant;
            newGenericParameter.IsCovariant = genericParameter.IsCovariant;
            newGenericParameter.IsNonVariant = genericParameter.IsNonVariant;
            //newGenericParameter.MetadataToken = genericParameter.MetadataToken;
        }

        // TODO: Move to clone factory
        private static void CloneMethodProperties(MethodDefinition interceptorMethod, MethodDefinition renamedMethod)
        {
            interceptorMethod.IsAbstract = renamedMethod.IsAbstract;
            interceptorMethod.IsAddOn = renamedMethod.IsAddOn;
            interceptorMethod.IsAssembly = renamedMethod.IsAssembly;
            interceptorMethod.IsCheckAccessOnOverride = renamedMethod.IsCheckAccessOnOverride;
            interceptorMethod.IsCompilerControlled = renamedMethod.IsCompilerControlled;
            interceptorMethod.IsFamily = renamedMethod.IsFamily;
            interceptorMethod.IsFamilyAndAssembly = renamedMethod.IsFamilyAndAssembly;
            interceptorMethod.IsFamilyOrAssembly = renamedMethod.IsFamilyOrAssembly;
            interceptorMethod.IsFinal = renamedMethod.IsFinal;
            interceptorMethod.IsFire = renamedMethod.IsFire;
            interceptorMethod.IsForwardRef = renamedMethod.IsForwardRef;
            interceptorMethod.IsGetter = renamedMethod.IsGetter;
            interceptorMethod.IsHideBySig = renamedMethod.IsHideBySig;
            interceptorMethod.IsIL = renamedMethod.IsIL;
            interceptorMethod.IsInternalCall = renamedMethod.IsInternalCall;
            interceptorMethod.IsManaged = renamedMethod.IsManaged;
            interceptorMethod.IsNative = renamedMethod.IsNative;
            interceptorMethod.IsNewSlot = renamedMethod.IsNewSlot;
            interceptorMethod.IsPInvokeImpl = renamedMethod.IsPInvokeImpl;
            interceptorMethod.IsPreserveSig = renamedMethod.IsPreserveSig;
            interceptorMethod.IsPrivate = renamedMethod.IsPrivate;
            interceptorMethod.IsPublic = renamedMethod.IsPublic;
            interceptorMethod.IsRemoveOn = renamedMethod.IsRemoveOn;
            interceptorMethod.IsReuseSlot = renamedMethod.IsReuseSlot;
            interceptorMethod.IsRuntime = renamedMethod.IsRuntime;
            interceptorMethod.IsRuntimeSpecialName = renamedMethod.IsRuntimeSpecialName;
            interceptorMethod.IsSetter = renamedMethod.IsSetter;
            interceptorMethod.IsSpecialName = renamedMethod.IsSpecialName;
            interceptorMethod.IsStatic = renamedMethod.IsStatic;
            interceptorMethod.IsSynchronized = renamedMethod.IsSynchronized;
            interceptorMethod.IsUnmanaged = renamedMethod.IsUnmanaged;
            interceptorMethod.IsUnmanagedExport = renamedMethod.IsUnmanagedExport;
            interceptorMethod.IsVirtual = renamedMethod.IsVirtual;
            interceptorMethod.NoInlining = renamedMethod.NoInlining;
            interceptorMethod.NoOptimization = renamedMethod.NoOptimization;
        }

        public virtual TypeReference Import(Type searchType)
        {
            return TypeInspector.AssemblyInspector.Import(searchType);
        }

        public virtual MethodReference Import(MethodDefinition method)
        {
            return TypeInspector.AssemblyInspector.Import(method);
        }

        public virtual TypeReference Import(TypeDefinition type)
        {
            return TypeInspector.AssemblyInspector.Import(type);
        }

        public virtual MethodReference Import(Type searchType, string methodName)
        {
            return TypeInspector.AssemblyInspector.Import(searchType, methodName);
        }

        public override string ToString()
        {
            return "{0}".FormatWith(Definition.FullName);
        }
    }
}