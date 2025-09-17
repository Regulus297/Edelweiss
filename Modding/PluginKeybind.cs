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
            { 0x01000000, "Escape" },
            { 0x01000001, "Tab" },
            { 0x01000002, "Backtab" },
            { 0x01000003, "Backspace" },
            { 0x01000004, "Return" },
            { 0x01000005, "Enter" },
            { 0x01000006, "Insert" },
            { 0x01000007, "Delete" },
            { 0x01000008, "Pause" },
            { 0x01000009, "Print" },
            { 0x0100000A, "SysReq" },
            { 0x0100000B, "Clear" },
            { 0x01000010, "Home" },
            { 0x01000011, "End" },
            { 0x01000012, "Left" },
            { 0x01000013, "Up" },
            { 0x01000014, "Right" },
            { 0x01000015, "Down" },
            { 0x01000016, "PageUp" },
            { 0x01000017, "PageDown" },
            { 0x01000020, "Shift" },
            { 0x01000021, "Control" },
            { 0x01000022, "Meta" },
            { 0x01000023, "Alt" },
            { 0x01000024, "CapsLock" },
            { 0x01000025, "NumLock" },
            { 0x01000026, "ScrollLock" },
            
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