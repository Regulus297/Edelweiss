using System;
using System.Collections.Generic;

namespace Edelweiss.Plugins
{
    public static class PluginVars
    {
        static Dictionary<string, Func<object>> vars = [];
        internal static void AddVar(string key, Func<object> getValue)
        {
            vars[key] = getValue;
        }

        public static object Get(string key)
        {
            if (!vars.TryGetValue(key, out Func<object> getValue))
                return null;

            return getValue();
        }
    }
}