using System;
using System.Collections.Generic;
using System.IO;

namespace Edelweiss.Utils
{
    public static class CelesteModLoader
    {
        public static Dictionary<string, string> texturePaths = [];
        public static bool LoadTextures(string graphicsPath)
        {
            MainPlugin.Instance.Logger.Log($"Loading textures from {graphicsPath}");
            graphicsPath = Path.Join(graphicsPath, "Graphics", "Atlases");
            if (!Directory.Exists(graphicsPath))
                return false;

            foreach (string file in Directory.GetFiles(graphicsPath, "*.png", SearchOption.AllDirectories))
            {
                string key = file.Substring(0, file.Length - 4).Substring(graphicsPath.Length + 1).Replace(Path.DirectorySeparatorChar, '/');
                texturePaths[key] = file;
            }

            return true;
        }
    }
}