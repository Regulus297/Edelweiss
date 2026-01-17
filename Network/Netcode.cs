using System.Collections.Generic;
using System.Reflection;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Network
{
    /// <summary>
    /// The class that registers and handles netcodes 
    /// </summary>
    public static class Netcode
    {
        /// <summary>
        /// The default netcode
        /// </summary>
        public const long NONE = 0x0;

        /// <summary>
        /// Registers python plugins. <br/>
        /// Parameters: <br/>
        /// - files: a list of string containing the paths of the python files to register
        /// </summary>
        public const long REGISTER_PYTHON_PLUGINS = 0x1;

        /// <summary>
        /// Registers a custom tab. <br/>
        /// Parameters: <br/>
        /// - name: the display name of the tab <br/>
        /// - internalName: the ID of the tab <br/>
        /// - json: the JSON representation of the tab
        /// </summary>
        public const long REGISTER_TAB = 0x2;

        /// <summary>
        /// Adds an item to a ZoomableView. <br/>
        /// Paramters: <br/>
        /// - widget: the tracker for the ZoomableView to add the item to <br/>
        /// - item: the JSON representation of the item <br/>
        /// - parent (optional): the name of the parent item of this item. Must be in the same ZoomableView
        /// </summary>
        public const long ADD_ITEM = 0x3;

        /// <summary>
        /// Modifies the shape data of a particular shape of an item. <br/>
        /// Parameters: <br/>
        /// - widget: the tracker for the ZoomableView the item is in <br/>
        /// - item: the name of the item the shape is in <br/>
        /// - index: the index of the shape in the item's shapes array <br/>
        /// - data: the modified data the shape should have
        /// </summary>
        public const long MODIFY_ITEM_SHAPE = 0x4;

        /// <summary>
        /// Modifies the item data of a particular item. <br/>
        /// Parameters: <br/>
        /// - widget: the tracker for the ZoomableView the item is in <br/>
        /// - item: the name of the item <br/>
        /// - data: the modified data the item should have
        /// </summary>
        public const long MODIFY_ITEM = 0x5;

        /// <summary>
        /// Changes the toolbar to the desired JSON representation. <br/>
        /// No parameters, only the JSON representation of the toolbar should be sent.
        /// </summary>
        public const long REGISTER_TOOLBAR = 0x6;

        /// <summary>
        /// Opens a popup widget with the passed JSON representation and treats it as a form. <br/>
        /// No parameters, only the JSON representation of the widget should be sent. <br/>
        /// Form JSON files must be loaded with <see cref="FormLoader.LoadForm(string, JObject)"/> before being sent
        /// </summary>
        public const long OPEN_POPUP_FORM = 0x7;

        /// <summary>
        /// Syncs a variable of a given name and value with the frontend. <br/>
        /// Parameters: <br/>
        /// - name: the name of the variable <br/>
        /// - value: the value of the variable
        /// </summary>
        public const long SYNC_VARIABLE = 0x8;

        /// <summary>
        /// Refreshes widgets with the given trackers <br/>
        /// Parameters: <br/>
        /// - widgets: the list of widget trackers that should be refreshed.
        /// </summary>
        public const long REFRESH_WIDGETS = 0x9;

        /// <summary>
        /// Opens a file dialog popup <br/>
        /// Parameters: <br/>
        /// - file: true if the user needs to select a file, false if directory <br/>
        /// - path: the path the dialog should default to <br/>
        /// - mode: "save" if user can select nonexistent paths, "load" if only existing paths can be chosen <br/>
        /// - pattern: glob patterns that can be selected, each separated by two semicolons: "*.bin;;*.png" <br/>
        /// - submit: event data when the form is submitted. Sends the path and matched pattern to the specified netcode
        /// </summary>
        public const long OPEN_FILE_DIALOG = 0xa;

        /// <summary>
        /// Deletes all items in the ZoomableView with the given tracker. <br/>
        /// Parameters: <br/>
        /// - widget: the tracker for the ZoomableView
        /// </summary>
        public const long CLEAR_VIEW = 0xb;

        /// <summary>
        /// Modifies the current loading screen. <br/>
        /// Parameters: <br/>
        /// - action: open, modify, close <br/>
        /// - name: used with the open action, creates and opens a loading screen with the name <br/>
        /// - progress: used with the modify action. Sets the current progress of the loading screen
        /// </summary>
        public const long MODIFY_LOADING_SCREEN = 0xc;

        /// <summary>
        /// Shows a popup with the given text. <br/>
        /// Parameters: <br/>
        /// - text: the text the popup contains
        /// - duration (optional): how long the popup lasts in milliseconds. 2000 by default.
        /// </summary>
        public const long SHOW_POPUP = 0xd;


        // Received packets

        /// <summary>
        /// 
        /// </summary>
        public const long KEY_PRESSED = -0x1;

        /// <summary>
        /// 
        /// </summary>
        public const long LIST_SELECTION_CHANGED = -0x2;

        /// <summary>
        /// 
        /// </summary>
        public const long TAB_CHANGED = -0x3;

        /// <summary>
        /// 
        /// </summary>
        public const long TOOL_BUTTON_PRESSED = -0x4;

        /// <summary>
        /// 
        /// </summary>
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