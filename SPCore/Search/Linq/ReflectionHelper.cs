using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SPCore.Search.Linq
{
    internal static class ReflectionHelper
    {
        //public const string WhereMethodName = "Where";
        //public const string OrderByMethodName = "OrderBy";
        //public const string SelectMethodName = "Select";
        //public const string FromMethodName = "From";
        public const string IndexerMethodName = "get_Item";
        public const string StartsWithMethodName = "StartsWith";
        public const string ContainsMethodName = "Contains";
        public const string IncludeTimeValue = "IncludeTimeValue";
        public const string CommonParameterName = "x";
        public const string Item = "Item";
        //public const string ToStringMethodName = "ToString";

        public const string GreaterThanMethodName = "op_GreaterThan";
        public const string GreaterThanOrEqualMethodName = "op_GreaterThanOrEqual";
        public const string LessThanMethodName = "op_LessThan";
        public const string LessThanOrEqualMethodName = "op_LessThanOrEqual";

        //public const string GreaterThanMethodSymbol = ">";
        //public const string GreaterThanOrEqualMethodSymbol = ">=";
        //public const string LessThanMethodSymbol = "<";
        //public const string LessThanOrEqualMethodSymbol = "<=";
        
        public static IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly,
            Type extendedType)
        {
            var query = from type in assembly.GetTypes()
                        where type.IsSealed && !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static
                            | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(ExtensionAttribute), false)
                        where method.GetParameters()[0].ParameterType == extendedType
                        select method;
            return query;
        }

        public static MethodInfo GetMethodInfo(Type type, string methodName)
        {
            return type.GetMethod(methodName);
        }
    }
}
