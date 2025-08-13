using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Edelweiss.Loenn;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities
{
    /// <summary>
    /// Class containing room data for the backend
    /// </summary>
    public class RoomData(JObject data) : ILuaConvertible
    {
        private static string DefaultJSON = """
        {
            "name": "a-01",
            "x": 0,
            "y": 0,
            "width": 40,
            "height": 23,
            "cameraOffsetX": 0.0,
            "cameraOffsetY": 0.0,
            "windPattern": "None",
            "colour": "0",
            "underwater": false,
            "space": false,
            "disableDownTransition": false,
            "checkpoint": false,
            "dark": false,
            "whisper": false,
            "musicLayer1": true,
            "musicLayer2": true,
            "musicLayer3": true,
            "musicLayer4": true,
            "music": "",
            "musicProgress": 0.0,
            "ambience": "",
            "ambienceProgress": 0.0
        }
        """;

        /// <summary>
        /// The default room data
        /// </summary>
        public static RoomData Default = new(JObject.Parse(DefaultJSON));

        /// <summary>
        /// </summary>
        public string name = data.Value<string>("name");

        /// <summary>
        /// 
        /// </summary>
        public int x = data.Value<int>("x") * 8;

        /// <summary>
        /// 
        /// </summary>
        public int y = data.Value<int>("y") * 8;

        /// <summary>
        /// 
        /// </summary>
        public int width = data.Value<int>("width") * 8;

        /// <summary>
        /// 
        /// </summary>
        public int height = data.Value<int>("height") * 8;

        /// <summary>
        /// 
        /// </summary>
        public bool musicLayer1 = data.Value<bool>("musicLayer1");

        /// <summary>
        /// 
        /// </summary>
        public bool musicLayer2 = data.Value<bool>("musicLayer2");

        /// <summary>
        /// 
        /// </summary>
        public bool musicLayer3 = data.Value<bool>("musicLayer3");

        /// <summary>
        /// 
        /// </summary>
        public bool musicLayer4 = data.Value<bool>("musicLayer4");

        /// <summary>
        /// 
        /// </summary>
        public float musicProgress = data.Value<float>("musicProgress");

        /// <summary>
        /// 
        /// </summary>
        public float ambienceProgress = data.Value<float>("ambienceProgress");

        /// <summary>
        /// 
        /// </summary>
        public bool dark = data.Value<bool>("dark");

        /// <summary>
        /// 
        /// </summary>
        public bool space = data.Value<bool>("space");

        /// <summary>
        /// 
        /// </summary>
        public bool underwater = data.Value<bool>("underwater");

        /// <summary>
        /// 
        /// </summary>
        public bool whisper = data.Value<bool>("whisper");

        /// <summary>
        /// 
        /// </summary>
        public bool disableDownTransition = data.Value<bool>("disableDownTransition");

        /// <summary>
        /// 
        /// </summary>
        public string music = data.Value<string>("music");

        /// <summary>
        /// 
        /// </summary>
        public string ambience = data.Value<string>("ambience");

        /// <summary>
        /// 
        /// </summary>
        public string windPattern = data.Value<string>("windPattern");

        /// <summary>
        /// 
        /// </summary>
        public string color = data.Value<string>("color");

        /// <summary>
        /// 
        /// </summary>
        public List<Entity> entities = [];

        /// <summary>
        /// Converts the room to a Lua table compatible with Loenn.
        /// </summary>
        public Table ToLuaTable(Script script)
        {
            Table table = new(script);
            foreach (FieldInfo field in typeof(RoomData).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (field.Name == "entities")
                {
                    table[field.Name] = entities.Select(e => e.ToLuaTable(script)).ToArray();
                    continue;
                }
                table[field.Name] = field.GetValue(this);
            }
            return table;
        }
    }
}