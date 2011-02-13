using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CryoAOP.Core.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace CryoAOP.Core
{
    public class MethodInspector
    {
        public readonly TypeInspector TypeInspector;
        public readonly MethodDefinition Definition;

        public MethodInspector(TypeInspector typeInspector, MethodDefinition definition)
        {
            TypeInspector = typeInspector;
            Definition = definition;
        }

        public void Write(string assemblyPath)
        {
            TypeInspector.AssemblyInspector.Write(assemblyPath);
        }

        public void InterceptMethod(string methodPrefix)
        {
            // Create new Method 
            var interceptorMethod = new MethodDefinition(Definition.Name, Definition.Attributes, Definition.ReturnType);
            
            // Rename existing method 
            var renamedMethod = Definition;
            renamedMethod.Name = "{0}{1}".FormatWith(methodPrefix, renamedMethod.Name);

            // Copy variables across 
            if (Definition.Body.HasVariables)
                foreach (var variable in Definition.Body.Variables)
                    interceptorMethod.Body.Variables.Add(variable);

            // Copy parameters across
            if (Definition.HasParameters)
                foreach (var parameter in Definition.Parameters)
                 interceptorMethod.Parameters.Add(parameter);   

            // Insert interceptor code
            
            // Interceptor: Insert variables 
            var type = typeof (Type);

            var v_0 = new VariableDefinition("V_0", TypeInspector.AssemblyInspector.Import(type));
            interceptorMethod.Body.Variables.Add(v_0);
            var v_1 = new VariableDefinition("V_1", TypeInspector.AssemblyInspector.Import(typeof (MethodInfo)));
            interceptorMethod.Body.Variables.Add(v_1);
            var v_2 = new VariableDefinition("V_2", TypeInspector.AssemblyInspector.Import(typeof (MethodInvocation)));
            interceptorMethod.Body.Variables.Add(v_2);
            var v_3 = new VariableDefinition("V_3", TypeInspector.AssemblyInspector.Import(typeof (Array)));
            interceptorMethod.Body.Variables.Add(v_3);

            // Interceptor: Insert interception IL
            
            // Intercptor: Resolve type meta data
            interceptorMethod.Nop();
            interceptorMethod.Ldtoken(TypeInspector.Definition);

            // Interceptor: Resolve type from handle
            const string type_GetTypeFromHandle = "GetTypeFromHandle";
            var getTypeFromHandleMethodReference = TypeInspector.AssemblyInspector.Import(type, type_GetTypeFromHandle);
            interceptorMethod.Call(getTypeFromHandleMethodReference);

            // Interceptor: Get the method info 
            interceptorMethod.Ldstr(interceptorMethod.Name);
            const string type_GetMethod = "GetMethod,String";
            var getMethodMethodReference = TypeInspector.AssemblyInspector.Import(type, type_GetMethod);
            interceptorMethod.Callvirt(getMethodMethodReference);

            // Interceptor: Move vars around
            interceptorMethod.Stloc_1();
            interceptorMethod.Ldloc_0();
            interceptorMethod.Ldloc_1();
            interceptorMethod.Ldc_I4_3();

            // Interceptor: Initialise object array for param values 
            var objectReference = TypeInspector.AssemblyInspector.Import(typeof (Object));
            interceptorMethod.Newarr(objectReference);
            foreach (var parameter in interceptorMethod.Parameters)
            {
                var argIndex = interceptorMethod.Parameters.IndexOf(parameter);
                interceptorMethod.Ldc_I4(argIndex);
                interceptorMethod.Ldarg((ushort)(argIndex + 1));
                if (parameter.ParameterType.IsValueType)
                    interceptorMethod.Box(parameter.ParameterType);
                interceptorMethod.Stelem_ref();
                interceptorMethod.Ldloc_3();
            }

            // Inteceptor: Initialise Method Invocation
            var methodInvocationTypRef = TypeInspector.AssemblyInspector.Import(typeof (MethodInvocation));
            var methodInvocationConstructor = methodInvocationTypRef.Resolve().Methods.Where(m => m.IsConstructor).First();
            var methodInvocationConstructorReference = TypeInspector.AssemblyInspector.Import(methodInvocationConstructor);
            interceptorMethod.NewObj(methodInvocationConstructorReference);

            // Interceptor: Call interceptor method 
            var methodInterceptorRef = TypeInspector.AssemblyInspector.Import(typeof (GlobalInterceptor), "HandleInvocation");
            interceptorMethod.Call(methodInterceptorRef);

            // Insert IL call from clone to renamed method
            interceptorMethod.Call(renamedMethod);
            interceptorMethod.Ret();

            // Add method to type
            Definition.DeclaringType.Methods.Add(interceptorMethod);
        }

        public override string ToString()
        {
            return "{0}".FormatWith(Definition.FullName);
        }
    }
}