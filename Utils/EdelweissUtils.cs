using System;
using System.Reflection;

namespace Edelweiss.Utils
{
    public static class EdelweissUtils
    {
        public static bool TryGetCustomAttribute<T>(this MemberInfo memberInfo, out T attr) where T : Attribute => memberInfo.TryGetCustomAttribute(false, out attr);
        public static bool TryGetCustomAttribute<T>(this MemberInfo memberInfo, bool inherit, out T attr) where T : Attribute
        {
            attr = memberInfo.GetCustomAttribute<T>(inherit);
            return attr != null;
        }
    }
}