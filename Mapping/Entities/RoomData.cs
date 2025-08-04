using System.Reflection;
using Edelweiss.Loenn;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities
{
    public class RoomData(JObject data): ILuaConvertible
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
        public static RoomData Default = new(JObject.Parse(DefaultJSON));

        public string name = data.Value<string>("name");
        public int x = data.Value<int>("x") * 8;
        public int y = data.Value<int>("y") * 8;
        public int width = data.Value<int>("width") * 8;
        public int height = data.Value<int>("height") * 8;
        public bool musicLayer1 = data.Value<bool>("musicLayer1");
        public bool musicLayer2 = data.Value<bool>("musicLayer2");
        public bool musicLayer3 = data.Value<bool>("musicLayer3");
        public bool musicLayer4 = data.Value<bool>("musicLayer4");

        public float musicProgress = data.Value<float>("musicProgress");
        public float ambienceProgress = data.Value<float>("ambienceProgress");

        public bool dark = data.Value<bool>("dark");
        public bool space = data.Value<bool>("space");
        public bool underwater = data.Value<bool>("underwater");
        public bool whisper = data.Value<bool>("whisper");
        public bool disableDownTransition = data.Value<bool>("disableDownTransition");
        public string music = data.Value<string>("music");
        public string ambience = data.Value<string>("ambience");

        public string windPattern = data.Value<string>("windPattern");
        public string color = data.Value<string>("color");

        public Table ToLuaTable(Script script)
        {
            Table table = new(script);
            foreach (FieldInfo field in typeof(RoomData).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                table[field.Name] = field.GetValue(this);
            }
            return table;
        }
    }
}