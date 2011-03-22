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

using System.Reflection;
using CryoAOP.Core.Exceptions;
using CryoAOP.Core.Extensions;

namespace CryoAOP.Core
{
    public class Invocation 
    {
        private readonly object[] parameterValues;

        public Invocation(System.Type type, MethodInfo method, params object[] parameterValues)
        {
            Type = type;
            Method = method;
            CanInvoke = true;
            this.parameterValues = parameterValues;
            InvocationType = InvocationType.Method;
            InvocationLifecycle = Core.InvocationLifecycleType.BeforeInvocation;
        }

        public Invocation(System.Type type, PropertyInfo property, MethodInfo method, params object[] parameterValues)
        {
            Type = type;
            Method = method;
            Property = property;
            CanInvoke = true;
            this.parameterValues = parameterValues;
            InvocationType = InvocationType.Property;
            InvocationLifecycle = Core.InvocationLifecycleType.BeforeInvocation;
        }

        public Invocation(object instance, System.Type type, MethodInfo method, params object[] parameterValues)
            : this(type, method, parameterValues)
        {
            Instance = instance;
        }

        public Invocation(object instance, System.Type type, PropertyInfo property, MethodInfo method, params object[] parameterValues)
            : this(type, property, method, parameterValues)
        {
            Instance = instance;
        }

        public System.Type Type { private set; get; }
        public object Instance { private set; get; }
        public bool CanInvoke { private set; get; }
        public MethodInfo Method { private set; get; }
        public PropertyInfo Property { private set; get; }
        public bool InvocationCancelled { private set; get; }
        public InvocationType InvocationType { private set; get; }
        public InvocationLifecycleType InvocationLifecycle { private set; get; }

        public ParameterInfo[] Parameters
        {
            get { return Method.GetParameters(); }
        }

        public object[] ParameterValues
        {
            get { return parameterValues; }
        }

        private object result = null;
        public object Result
        {
            get { return result; }
            set
            {
                result = value;
                ValidateResult();
            }
        }

        public virtual void ContinueInvocation()
        {
            CanInvoke = false;
            InvocationLifecycle = InvocationLifecycleType.AfterInvocation;
        }

        public virtual void CancelInvocation()
        {
            CanInvoke = false;
            InvocationCancelled = InvocationLifecycle == InvocationLifecycleType.BeforeInvocation;
        }

        protected virtual void ValidateResult()
        {
            if (result != null && Method.ReturnType == typeof(void))
                throw new MethodSignatureViolationException(
                    "You have assigned and incorrect type a return type! Please use explicit cast. Got '{0}' but expected 'Void'.",
                    result.GetType().FullName);

            if (result != null
                && Method.ReturnType != null
                && !Method.ReturnType.IsAssignableFrom(result.GetType())
                && !Method.ReturnType.IsGenericParameter)
                throw new MethodSignatureViolationException(
                    "You have assigned and incorrect type a return type! Please use explicit cast. Got '{0}' but expected '{1}'.",
                    result.GetType().FullName, Method.ReturnType.FullName);
        }

        public override string ToString()
        {
            return string.Format("InvocationType: {7}, InvocationLifecycle: {4}, Type: {2}, Property: {6}, Method: {3}, CanInvoke: {5}, ParameterValues: {1}, Result: {0}", Result, parameterValues.JoinWith(","), Type, Method, InvocationLifecycle, CanInvoke, Property, InvocationType);
        }
    }
}