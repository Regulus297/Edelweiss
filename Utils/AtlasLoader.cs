using System;
using System.IO;
using System.Linq;
using Edelweiss.Plugins;

namespace Edelweiss.Utils
{
    /// <summary>
    /// The class responsible for loading atlases
    /// </summary>
    public static class AtlasLoader
    {
        /// <summary>
        /// Loads a .meta atlas file and adds the sprites and sprite data to the CelesteModLoader dictionaries
        /// </summary>
        /// <param name="filePath"></param>
        public static void LoadAtlasMetaFile(string filePath)
        {
            using FileStream stream = File.Open(filePath, FileMode.Open);
            using BinaryReader reader = new(stream);
            reader.ReadInt32();
            reader.ReadString();
            reader.ReadInt32();
            short dataAmount = reader.ReadInt16();
            for (int i = 0; i < dataAmount; i++)
            {
                string dataName = reader.ReadString();
                if (!MainPlugin.Instance.CacheExists(dataName + ".bmp"))
                {
                    LoadAtlasTexture(Path.Join(Path.GetDirectoryName(filePath), dataName + ".data"));
                }
                else
                {
                    MainPlugin.Instance.Logger.Log($"Loading atlas {dataName} from cache.");
                }

                short imageAmount = reader.ReadInt16();
                for (int j = 0; j < imageAmount; j++)
                {
                    string name = reader.ReadString().Replace('\\', '/');
                    short x = reader.ReadInt16();
                    short y = reader.ReadInt16();
                    short w = reader.ReadInt16();
                    short h = reader.ReadInt16();
                    short x1 = reader.ReadInt16();
                    short y1 = reader.ReadInt16();
                    short w1 = reader.ReadInt16();
                    short h1 = reader.ReadInt16();

                    string key = Path.GetFileNameWithoutExtension(filePath) + "/" + name;
                    CelesteModLoader.texturePaths[key] = MainPlugin.Instance.CachePath(dataName + ".bmp");
                    CelesteModLoader.textureDataCache[key] = new TextureData
                    {
                        width = w,
                        height = h,
                        atlasX = x,
                        atlasY = y,
                        atlasWidth = w,
                        atlasHeight = h,
                        atlasOffsetX = x1,
                        atlasOffsetY = y1,
                        paddedWidth = w1,
                        paddedHeight = h1
                    };
                }
            }
        }

        /// <summary>
        /// Loads an atlas texture (.data file) and then caches it as a .bmp file.
        /// </summary>
        /// <param name="path">The absolute path to the atlas texture</param>
        public unsafe static void LoadAtlasTexture(string path)
        {
            // Much of this method was copied from Monocle's VirtualTexture.Reload, specifically the ".data" case. 
            // It has been adapted to generate BGRA data to save in bitmaps instead of the usual RGBA.
            byte[] bytes = new byte[524288];
            using (FileStream fileStream = File.OpenRead(path))
            {
                fileStream.Read(bytes, 0, 524288);
                byte[] buffer = new byte[67108864];
                int num2 = 0;
                int num3 = BitConverter.ToInt32(bytes, num2);
                int num4 = BitConverter.ToInt32(bytes, num2 + 4);
                bool flag = bytes[num2 + 8] == 1;
                num2 += 9;
                int num5 = num3 * num4 * 4;
                int num6 = 0;
                fixed (byte* ptr3 = bytes)
                {
                    fixed (byte* ptr4 = buffer)
                    {
                        while (num6 < num5)
                        {
                            int num7 = ptr3[num2] * 4;
                            if (flag)
                            {
                                byte b = ptr3[num2 + 1];
                                if (b > 0)
                                {
                                    ptr4[num6] = ptr3[num2 + 2];
                                    ptr4[num6 + 1] = ptr3[num2 + 3];
                                    ptr4[num6 + 2] = ptr3[num2 + 4];
                                    ptr4[num6 + 3] = b;
                                    num2 += 5;
                                }
                                else
                                {
                                    ptr4[num6] = 0;
                                    ptr4[num6 + 1] = 0;
                                    ptr4[num6 + 2] = 0;
                                    ptr4[num6 + 3] = 0;
                                    num2 += 2;
                                }
                            }
                            else
                            {
                                ptr4[num6] = ptr3[num2 + 1];
                                ptr4[num6 + 1] = ptr3[num2 + 2];
                                ptr4[num6 + 2] = ptr3[num2 + 3];
                                ptr4[num6 + 3] = byte.MaxValue;
                                num2 += 4;
                            }
                            if (num7 > 4)
                            {
                                int k = num6 + 4;
                                for (int num8 = num6 + num7; k < num8; k += 4)
                                {
                                    ptr4[k] = ptr4[num6 + 0];
                                    ptr4[k + 1] = ptr4[num6 + 1];
                                    ptr4[k + 2] = ptr4[num6 + 2];
                                    ptr4[k + 3] = ptr4[num6 + 3];
                                }
                            }
                            num6 += num7;
                            if (num2 > 524256)
                            {
                                int num9 = 524288 - num2;
                                for (int l = 0; l < num9; l++)
                                {
                                    ptr3[l] = ptr3[num2 + l];
                                }
                                fileStream.Read(bytes, num9, 524288 - num9);
                                num2 = 0;
                            }
                        }
                    }
                }
                Bitmap bitmap = new(num3, num4, buffer);
                byte[] data = bitmap.GetBytes();
                using Stream stream = MainPlugin.Instance.CreateCache(Path.GetFileNameWithoutExtension(path) + ".bmp");
                using BinaryWriter writer = new(stream);
                writer.Write(data);
            }
        }
    }
}