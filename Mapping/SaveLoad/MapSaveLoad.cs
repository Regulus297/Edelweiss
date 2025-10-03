using System;
using System.Collections.Generic;
using System.IO;
using Edelweiss.Mapping.Entities;

namespace Edelweiss.Mapping.SaveLoad
{
    /// <summary>
    /// Class responsible for saving and loading Celeste maps
    /// </summary>
    public static class MapSaveLoad
    {
        /// <summary>
        /// A string lookup table for the current map being saved
        /// </summary>
        public static StringLookup stringLookup = [];

        /// <summary>
        /// A lookup table for the current map being loaded
        /// </summary>
        public static List<string> backwardsLookup = [];

        /// <summary>
        /// Saves a map to the given path
        /// </summary>
        /// <param name="map">The map to save</param>
        /// <param name="filePath">The path to save the file to, including the .bin extension</param>
        public static void SaveMap(MapData map, string filePath)
        {
            using FileStream stream = File.Open(filePath, FileMode.Create);
            using BinaryWriter writer = new BinaryWriter(stream);

            // Write header
            writer.Write("CELESTE MAP");
            writer.Write(Path.GetFileNameWithoutExtension(filePath));

            // Create lookup
            stringLookup.Clear();
            CreateLookup(map);
            writer.Write((short)stringLookup.Count);
            foreach (var item in stringLookup)
            {
                writer.Write(item.Key);
            }

            // Write content
            map.Encode(writer);
            writer.Flush();
        }

        /// <summary>
        /// Creates a string lookup for the given map
        /// </summary>
        /// <param name="map"></param>
        public static void CreateLookup(MapData map)
        {
            stringLookup.Add("innerText");
            map.AddToLookup(stringLookup);
        }

        /// <summary>
        /// Writes a value's type and the value itself to the writer
        /// </summary>
        public static void WriteValue(this BinaryWriter writer, object value)
        {
            if (value is bool b)
            {
                writer.Write((byte)0);
                writer.Write(b);
            }
            else if (value is byte by)
            {
                writer.Write((byte)1);
                writer.Write(by);
            }
            else if (value is short sh)
            {
                writer.Write((byte)2);
                writer.Write(sh);
            }
            else if (value is int i)
            {
                writer.Write((byte)3);
                writer.Write(i);
            }
            else if (value is float f)
            {
                writer.Write((byte)4);
                writer.Write(f);
            }
            else if (value is double d)
            {
                writer.Write((byte)4);
                writer.Write((float)d);
            }
            else if (value is string s)
            {
                if (stringLookup.ContainsKey(s))
                {
                    writer.Write((byte)5);
                    writer.WriteLookupString(s);
                }
                else
                {
                    writer.Write((byte)6);
                    writer.Write(s);
                }
            }
        }

        /// <summary>
        /// Reads a value from the start of the reader
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="type">The type of data. 0 = bool, 1 = byte, 2 = short, 3 = int, 4 = float, 5 = lookup string, 6 = string, 7 = RLE string</param>
        /// <returns></returns>
        public static object ReadValue(this BinaryReader reader, out byte type)
        {
            type = reader.ReadByte();
            return type switch
            {
                0 => reader.ReadBoolean(),
                1 => reader.ReadByte(),
                2 => reader.ReadInt16(),
                3 => reader.ReadInt32(),
                4 => reader.ReadSingle(),
                5 => backwardsLookup[reader.ReadInt16()],
                6 => reader.ReadString(),
                7 => reader.ReadRLEString(),
                _ => null
            };
        }

        /// <summary>
        /// Writes the specified string's lookup value to the writer
        /// </summary>
        public static void WriteLookupString(this BinaryWriter writer, string value)
        {
            writer.Write(stringLookup[value]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static string ReadLookupString(this BinaryReader reader) => backwardsLookup[reader.ReadInt16()];

        /// <summary>
        /// Writes an attribute (key and value) to the writer
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void WriteAttribute(this BinaryWriter writer, string name, object value)
        {
            writer.WriteLookupString(name);
            writer.WriteValue(value);
        }

        /// <summary>
        /// Writes a run-length encoded string to the writer
        /// </summary>
        public static void WriteRLEString(this BinaryWriter writer, string value)
        {
            writer.Write((byte)7);
            List<byte> bytes = [];
            for (int i = 0; i < value.Length; i++)
            {
                byte count = 1;
                char c;
                for (c = value[i]; i + 1 < value.Length && value[i + 1] == c; i++)
                {
                    if (count >= byte.MaxValue)
                    {
                        break;
                    }
                    count++;
                }
                bytes.Add(count);
                bytes.Add((byte)c);
            }
            writer.Write((short)bytes.Count);
            writer.Write(bytes.ToArray());
        }

        /// <summary>
        /// Reads a run-length encoded string from the reader
        /// </summary>
        public static string ReadRLEString(this BinaryReader reader) {
            short length = reader.ReadInt16();
            byte[] bytes = reader.ReadBytes(length);
            string output = "";
            for (int i = 0; i < length; i += 2)
            {
                byte runLength = bytes[i];
                char c = (char)bytes[i + 1];
                for (int _ = 0; _ < runLength; _++)
                {
                    output += c;
                }
            }
            return output;
        }
    }
}