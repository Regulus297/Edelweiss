using System;
using System.Reflection;

namespace Edelweiss.Utils
{
    public static class EdelweissUtils
    {
        public static bool TryGetCustomAttribute<T>(this MemberInfo memberInfo, out T attr) where T : Attribute
        {
            attr = memberInfo.GetCustomAttribute<T>();
            return attr != null;
        }
    }
}