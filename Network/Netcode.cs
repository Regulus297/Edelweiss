using System.Collections.Generic;
using System.Reflection;

namespace Edelweiss.Network
{
    public static class Netcode
    {
        public const long NONE = 0x0;
        public const long QUIT = 0x1;
        public const long REGISTER_PYTHON_PLUGINS = 0x2;
        public const long REGISTER_SCENE = 0x3;
        public const long REGISTER_JSON_SCENE = 0x4;
        public const long ADD_ITEM = 0x5;
        public const long MODIFY_ITEM_SHAPE = 0x6;
        public const long REGISTER_TOOLBAR = 0x7;
        public const long OPEN_POPUP_FORM = 0x8;


        // Received packets
        public const long BUTTON_PRESSED = -0x1;
        public const long LIST_SELECTION_CHANGED = -0x2;
        public const long TAB_CHANGED = -0x3;
        public const long TOOL_BUTTON_PRESSED = -0x4;
        public const long FORM_SUBMITTED = -0x5;

        // Dynamic Netcode generation
        internal static Dictionary<string, long> codes = [];
        internal static long maximum = 0;
        internal static long minimum = 0;

        internal static void Initialize()
        {
            foreach (FieldInfo field in typeof(Netcode).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (field.IsLiteral && field.FieldType == typeof(long))
                {
                    long value = (long)field.GetValue(null);
                    if (value > maximum)
                    {
                        maximum = value;
                    }
                    if (value < minimum)
                    {
                        minimum = value;
                    }
                    codes[field.Name] = value;
                }
            }
        }

        internal static long CreateNetcode(string name, bool positive)
        {
            if (positive)
                codes[name] = ++maximum;
            else
                codes[name] = --minimum;
            return codes[name];
        }

        /// <summary>
        /// Gets the netcode of a certain name
        /// </summary>
        /// <param name="name">The name of the netcode</param>
        public static long Get(string name)
        {
            if (!codes.TryGetValue(name, out long value))
                return NONE;
            return value;
        }
    }
}