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
using CryoAOP.Core;
using CryoAOP.Core.Extensions;

namespace CryoAOP.TestAssembly
{
    public class MethodInterceptorTarget
    {
        private void privateMethodThatBreaksReflectionWhenTryingToGetMethodInfoUsingGetMethod(int a, double b, string c)
        {
        }

        public void HavingMethodWithNoArgsAndNoReturnType()
        {
        }

        public int HavingMethodWithNoArgsAndInt32ReturnType()
        {
            return 1;
        }

        public void HavingMethodWithArgsAndNoReturnType(int arg1, string arg2, double arg3)
        {
        }

        public string HavingMethodWithArgsAndStringReturnType(int arg1, string arg2, double arg3)
        {
            var result = "{0}, {1}, {2}".FormatWith(arg1, arg2, arg3);
            return result;
        }

        public void HavingMethodWithClassArgsAndNoReturnType(MethodInterceptorTargetParameter arg1)
        {
        }

        public MethodInterceptorTargetParameter HavingMethodWithClassArgsAndClassReturnType(MethodInterceptorTargetParameter arg1)
        {
            return arg1;
        }

        public void GenericMethod<T>()
        {
        }

        public void GenericMethodWithGenericParameters<T>(T t)
        {
        }

        public T GenericMethodWithGenericParametersAndGenericReturnType<T>(T t)
        {
            return t;
        }

        public void GenericMethodWithGenericParametersAndValueTypeArgs<T>(T t, int i, double j)
        {
        }

        public double GenericMethodWithGenericParamsAndValueReturnType<T>(T t, int i, double j)
        {
            return j;
        }

        public void GenericMethodWithInvertedParams<T>(int i, T t)
        {
        }

        public int GenericMethodWithInvertedParamsAndValueReturnType<T>(int i, T t)
        {
            return i;
        }

        public void GenericMethodWithTwoGenericParameters<I, J>(I i, J j)
        {
        }

        public static void StaticMethodWithNoArgsAndNoReturnType()
        {
        }

        public static void StaticMethodWithArgsAndNoReturnType(int i)
        {
        }

        public static void StaticMethodWithGenericAndValueTypeArgsAndNoReturnType<T>(int i, T t)
        {
        }

        public static int StaticMethodWithGenericAndValueTypeArgsAndValueReturnType<T>(int i, T t)
        {
            return i;
        }

        public static T StaticMethodWithGenericAndValueTypeArgsAndGenericReturnType<T>(int i, T t)
        {
            return t;
        }

        public void InterceptMethod()
        {
        }

        public void CallToIntercept()
        {
            InterceptMethod();
        }
    }
}