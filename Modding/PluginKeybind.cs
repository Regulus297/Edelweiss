using System;
using System.Collections.Generic;
using System.Linq;
using Edelweiss.RegistryTypes;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Plugins
{
    /// <summary>
    /// A saveable preference that contains keybinds
    /// </summary>
    public abstract class PluginKeybind : PluginSaveablePreference

    {
        /// <summary>
        /// A dictionary relating Qt Keycodes to their names
        /// </summary>
        public static readonly Dictionary<int, string> QtKeyNames = new Dictionary<int, string>
        {
            // Common control keys
            { 0x01000000, "unknown" },
            { 0x01000001, "Escape" },
            { 0x01000002, "Tab" },
            { 0x01000003, "Backtab" },
            { 0x01000004, "Backspace" },
            { 0x01000005, "Return" },
            { 0x01000006, "Enter" },
            { 0x01000007, "Insert" },
            { 0x01000008, "Delete" },
            { 0x01000009, "Pause" },
            { 0x0100000A, "Print" },
            { 0x0100000B, "SysReq" },
            { 0x0100000C, "Clear" },
            { 0x0100000D, "Home" },
            { 0x0100000E, "End" },
            { 0x01000010, "Left" },
            { 0x01000011, "Up" },
            { 0x01000012, "Right" },
            { 0x01000013, "Down" },
            { 0x01000020, "PageUp" },
            { 0x01000021, "PageDown" },
            { 0x01000022, "Shift" },
            { 0x01000023, "Control" },
            { 0x01000024, "Alt" },
            { 0x01000025, "Meta" },
            { 0x01000026, "CapsLock" },
            { 0x01000027, "NumLock" },
            { 0x01000028, "ScrollLock" },
            
            // Function keys
            { 0x01000030, "F1" },
            { 0x01000031, "F2" },
            { 0x01000032, "F3" },
            { 0x01000033, "F4" },
            { 0x01000034, "F5" },
            { 0x01000035, "F6" },
            { 0x01000036, "F7" },
            { 0x01000037, "F8" },
            { 0x01000038, "F9" },
            { 0x01000039, "F10" },
            { 0x0100003A, "F11" },
            { 0x0100003B, "F12" },
            { 0x0100003C, "F13" },
            { 0x0100003D, "F14" },
            { 0x0100003E, "F15" },
            { 0x0100003F, "F16" },
            { 0x01000040, "F17" },
            { 0x01000041, "F18" },
            { 0x01000042, "F19" },
            { 0x01000043, "F20" },
            
            // Printable ASCII / letters and digits
            { 'A', "A" },
            { 'B', "B" },
            { 'C', "C" },
            { 'D', "D" },
            { 'E', "E" },
            { 'F', "F" },
            { 'G', "G" },
            { 'H', "H" },
            { 'I', "I" },
            { 'J', "J" },
            { 'K', "K" },
            { 'L', "L" },
            { 'M', "M" },
            { 'N', "N" },
            { 'O', "O" },
            { 'P', "P" },
            { 'Q', "Q" },
            { 'R', "R" },
            { 'S', "S" },
            { 'T', "T" },
            { 'U', "U" },
            { 'V', "V" },
            { 'W', "W" },
            { 'X', "X" },
            { 'Y', "Y" },
            { 'Z', "Z" },
            { '0', "0" },
            { '1', "1" },
            { '2', "2" },
            { '3', "3" },
            { '4', "4" },
            { '5', "5" },
            { '6', "6" },
            { '7', "7" },
            { '8', "8" },
            { '9', "9" },
            
            // Symbols
            { ' ', "Space" },
            { '!', "Exclam" },
            { '"', "QuoteDbl" },
            { '#', "NumberSign" },
            { '$', "Dollar" },
            { '%', "Percent" },
            { '&', "Ampersand" },
            { '\'', "Apostrophe" },
            { '(', "ParenLeft" },
            { ')', "ParenRight" },
            { '*', "Asterisk" },
            { '+', "Plus" },
            { ',', "Comma" },
            { '-', "Minus" },
            { '.', "Period" },
            { '/', "Slash" },
            { ':', "Colon" },
            { ';', "Semicolon" },
            { '<', "Less" },
            { '=', "Equal" },
            { '>', "Greater" },
            { '?', "Question" },
            { '@', "At" },
            { '[', "BracketLeft" },
            { '\\', "Backslash" },
            { ']', "BracketRight" },
            { '^', "AsciiCircum" },
            { '_', "Underscore" },
            { '{', "BraceLeft" },
            { '}', "BraceRight" },
            { '|', "Bar" },
            { '~', "AsciiTilde" }
        };

        /// <summary>
        /// A dictionary relating Qt Key names to their keycodes
        /// </summary>
        public static Dictionary<string, int> ReverseQtKeyNames = QtKeyNames.Select(t => new KeyValuePair<string, int>(t.Value, t.Key)).ToDictionary();

        /// <summary>
        /// The keys that this keybind is bound to by default
        /// </summary>
        public abstract List<int> DefaultBindings { get; }

        /// <summary>
        /// The keys that this keybind is currently bound to
        /// </summary>
        public List<int> CurrentBindings => (Value as JArray).Select(t => (int)t).ToList();

        /// <summary>
        /// The event invoked when this keybind is pressed
        /// </summary>
        public event Action OnPressed;

        internal void Press()
        {
            OnPressed?.Invoke();
        }

        /// <inheritdoc/>
        public sealed override void SetDefaultValue()
        {
            Value = JArray.FromObject(DefaultBindings);
        }

        /// <summary>
        /// Adds a listener that is invoked whenever the keybind of the target type is pressed
        /// </summary>
        /// <typeparam name="T">The type of keybind to listen to</typeparam>
        /// <param name="callback">The action called when the keybind is pressed</param>
        public static void AddListener<T>(Action callback) where T : PluginKeybind
        {
            Registry.registry[typeof(PluginSaveablePreference)].GetValue<T>().OnPressed += callback;
        }
    }
}