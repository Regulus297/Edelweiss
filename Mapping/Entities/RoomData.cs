using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Edelweiss.Loenn;
using Edelweiss.Mapping.SaveLoad;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Entities
{
    /// <summary>
    /// Class containing room data for the backend
    /// </summary>
    public class RoomData(JObject data) : ILuaConvertible, IMapSaveable
    {
        internal static string DefaultJSON = """
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
        public static readonly RoomData Default = new(JObject.Parse(DefaultJSON));

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
        public string color = data.Value<string>("colour");

        /// <summary>
        /// The foreground tiles in the room
        /// </summary>
        public TileMatrix fgTileData = new TileMatrix(data.Value<int>("width"), data.Value<int>("height"));

        /// <summary>
        /// The background tiles in the room
        /// </summary>
        public TileMatrix bgTileData = new TileMatrix(data.Value<int>("width"), data.Value<int>("height"));

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, Entity> entities = [];

        /// <summary>
        /// The map this room belongs to
        /// </summary>
        public MapData map;

        /// <summary>
        /// Converts the room to a Lua table compatible with Loenn.
        /// </summary>
        public Table ToLuaTable(Script script)
        {
            Table table = new(script);
            foreach (FieldInfo field in typeof(RoomData).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (field.Name == nameof(map) || field.Name == nameof(fgTileData) || field.Name == nameof(bgTileData))
                {
                    continue;
                }
                if (field.Name == "entities")
                {
                    table[field.Name] = entities.Select(e => e.Value.ToLuaTable(script)).ToArray();
                    continue;
                }
                table[field.Name] = field.GetValue(this);
            }
            return table;
        }

        /// <summary>
        /// Redraws all entities in the room
        /// </summary>
        public void RedrawEntities()
        {
            foreach (Entity entity in entities.Values)
            {
                entity.Draw($"{name}/{name}:{entity._id}", 0, true);
            }
        }

        /// <summary>
        /// Removes an entity from the room
        /// </summary>
        /// <param name="entity"></param>
        public void RemoveEntity(Entity entity)
        {
            entities.Remove(entity._id);
        }

        /// <inheritdoc/>
        public void AddToLookup(StringLookup lookup)
        {
            lookup.Add("level", "solids", "bg", "fgtiles", "bgtiles", "objtiles", "offsetX", "offsetY", "triggers");
            foreach (FieldInfo field in typeof(RoomData).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (field.Name == nameof(map)  || field.Name == nameof(fgTileData) || field.Name == nameof(bgTileData))
                {
                    continue;
                }
                lookup.Add(field.Name == "color" ? "c" : field.Name);
                object value = field.GetValue(this);
                if (value is string s)
                {
                    lookup.Add(s);
                }
            }
            foreach (Entity entity in entities.Values)
            {
                entity.AddToLookup(lookup);
            }
        }

        /// <inheritdoc/>
        public void Encode(BinaryWriter writer)
        {
            writer.WriteLookupString("level");
            Dictionary<string, object> fields = [];
            foreach (FieldInfo field in typeof(RoomData).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (field.Name == nameof(map) || field.Name == nameof(entities) || field.Name == nameof(fgTileData) || field.Name == nameof(bgTileData))
                {
                    continue;
                }
                fields[field.Name == "color" ? "c" : field.Name] = field.Name == "color" ? int.Parse(field.GetValue(this).ToString()) : field.GetValue(this);
            }

            writer.Write((byte)fields.Count); // Attr count
            foreach (var field in fields)
            {
                writer.WriteAttribute(field.Key, field.Value);
            }

            writer.Write((short)4); // Child count

            // entities child
            writer.WriteLookupString("entities");
            writer.Write((byte)2); // Attr count
            writer.WriteAttribute("offsetX", 0);
            writer.WriteAttribute("offsetY", 0);

            writer.Write((short)entities.Count); // Child count
            foreach (Entity entity in entities.Values)
            {
                entity.Encode(writer);
            }

            // triggers
            writer.WriteLookupString("triggers");
            writer.Write((byte)0);
            writer.Write((short)0);

            // solids
            writer.WriteLookupString("solids");
            writer.Write((byte)1);
            writer.WriteLookupString("innerText");
            writer.WriteRLEString(fgTileData.Format());
            writer.Write((short)0);

            // bg
            writer.WriteLookupString("bg");
            writer.Write((byte)1);
            writer.WriteLookupString("innerText");
            writer.WriteRLEString(fgTileData.Format());
            writer.Write((short)0);
        }

        /// <inheritdoc/>
        public void Decode(MapElement element)
        {
            foreach (FieldInfo field in typeof(RoomData).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (field.Name == nameof(map) || field.Name == nameof(entities) || field.Name == nameof(fgTileData) || field.Name == nameof(bgTileData))
                {
                    continue;
                }
                string name = field.Name == "color" ? "c" : field.Name;
                bool progress = field.Name == "musicProgress" || field.Name == "ambienceProgress";
                element.AttrIf<object>(name, v => field.SetValue(this, progress ? (string.IsNullOrEmpty(v.ToString()) ? 0f : float.Parse(v.ToString())) : (name == "c" ? v.ToString() : v)));
            }

            fgTileData = new TileMatrix(width / 8, height / 8);
            bgTileData = new TileMatrix(width / 8, height / 8);

            foreach (MapElement child in element)
            {
                if (child.Name == "entities")
                {
                    foreach (MapElement entityElement in child)
                    {
                        Entity entity = new Entity("", "");
                        entity.Decode(entityElement);
                        entity.entityRoom = this;
                        entities[entity._id] = entity;
                    }
                }
                else if (child.Name == "solids")
                {
                    child.AttrIf<string>("innerText", v => fgTileData.Decode(v));
                }
                else if (child.Name == "bg")
                {
                    child.AttrIf<string>("innerText", v => bgTileData.Decode(v));
                }
            }
        }

        internal static string GetColor(string choice)
        {
            return choice switch
            {
                "1" => "#ed6a1f",
                "2" => "#88e33d",
                "3" => "#3be3dd",
                "4" => "#227ac7",
                "5" => "#a723b0",
                "6" => "#d61aa1",
                _ => "#ffffff"
            };
        }
    }
}