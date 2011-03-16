using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CryoAOP.Core.Exceptions;

namespace CryoAOP.Core.Extensions
{
    public static class ReflectionExtensions
    {
        #region KeyIncrementType enum

        public enum KeyIncrementType
        {
            Increment,
            DontIncrement
        }

        #endregion

        public static System.Type FindType(this System.Reflection.Assembly assembly, string typeName)
        {
            var type = assembly.GetTypes().Where(t => t.FullName.ToLower().EndsWith(typeName.ToLower())).FirstOrDefault();
            if (type == null)
                throw new TypeNotFoundException("Could not find type '{0}' in '{1}'", typeName, assembly.FullName);
            return type;
        }

        public static object AutoInstanceInvoke(this MethodInfo method, params object[] args)
        {
            var type = method.DeclaringType;
            object returnValue = null;
            if (!method.IsStatic)
            {
                var instance = type.Assembly.CreateInstance(type.FullName);
                returnValue = method.Invoke(instance, args);
            }
            else
            {
                returnValue = method.Invoke(null, args);
            }
            return returnValue;
        }

        public static string InstanceDiff(this object left, object right)
        {
            var instanceDiffBuilder = new StringBuilder();
            var instanceDiffDict = new Dictionary<string, string>();

            CompareValues(
                left, left.GetType().GetFields(),
                right, right.GetType().GetFields())
                .ForEach(keyValue =>
                             {
                                 if (keyValue.Value != null)
                                     instanceDiffDict.Add(keyValue.Key, keyValue.Value);
                             });

            CompareValues(
                left, left.GetType().GetProperties(),
                right, right.GetType().GetProperties())
                .ForEach(keyValue =>
                             {
                                 if (keyValue.Value != null)
                                     instanceDiffDict.Add(keyValue.Key, keyValue.Value);
                             });

            foreach (var valueKey in instanceDiffDict.Keys)
                instanceDiffBuilder.AppendLine("{0} => {1}\r\n".FormatWith(valueKey, instanceDiffDict[valueKey]));

            return instanceDiffBuilder.ToString();
        }

        private static Dictionary<string, string> CompareValues<T>(
            object leftInstance, IEnumerable<T> leftMembers,
            object rightInstance, IEnumerable<T> rightMembers) where T : MemberInfo
        {
            var counter = 0;
            var leftType = leftInstance.GetType();
            var rightType = rightInstance.GetType();
            var instanceDiffDict = new Dictionary<string, string>();

            foreach (var leftMember in leftMembers)
            {
                var leftKey =
                    !instanceDiffDict.ContainsKey(GetKey(leftType.FullName, leftMember.Name))
                        ? GetKey(leftType.FullName, leftMember.Name)
                        : GetNewKey(
                            leftType.FullName,
                            leftMember.Name,
                            ref counter);


                instanceDiffDict.Add(leftKey, null);

                foreach (var rightMember in rightMembers)
                {
                    var rightKeyA = GetKey(rightType.FullName, rightMember.Name);

                    var rightKeyB =
                        GetNewKey(
                            rightType.FullName,
                            rightMember.Name,
                            ref counter,
                            KeyIncrementType.DontIncrement);

                    if (leftKey != rightKeyA && leftKey != rightKeyB) continue;
                    var leftInstanceValue = GetFieldOrPropertyValue(leftMember, leftInstance);
                    var rightInstanceValue = GetFieldOrPropertyValue(rightMember, rightInstance);
                    CompareValuesAndCompileResult(leftKey, instanceDiffDict, leftInstanceValue, rightInstanceValue);
                }
            }

            return instanceDiffDict;
        }

        public static object GetFieldOrPropertyValue(MemberInfo memberInfo, object instance)
        {
            if (memberInfo.GetType() == typeof (FieldInfo))
                return ((FieldInfo) memberInfo).GetValue(instance);
            return ((PropertyInfo) memberInfo).GetValue(instance, null);
        }

        private static string GetKey(string leftTypeFullName, string leftMember)
        {
            return "{0}.{1}".FormatWith(leftTypeFullName, leftMember);
        }

        private static string GetNewKey(string leftTypeFullName, string leftMember, ref int counter, KeyIncrementType incType = KeyIncrementType.Increment)
        {
            return "{0}.{1}.{2}"
                .FormatWith(
                    leftTypeFullName,
                    leftMember,
                    incType == KeyIncrementType.Increment
                        ? counter++
                        : counter);
        }

        private static void CompareValuesAndCompileResult(string valueKey, Dictionary<string, string> results, object leftValue, object rightValue)
        {
            if (leftValue != null
                && rightValue != null
                && leftValue.Equals(rightValue))
            {
                results[valueKey] = null;// "{0} equals {1}".FormatWith(leftValue, rightValue);
            }
            else
            {
                results[valueKey] = "{0} DOES NOT equal {1}".FormatWith(leftValue, rightValue);
            }
        }
    }
}