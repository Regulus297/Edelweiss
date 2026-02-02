using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace Edelweiss.Utils
{
    public static class EdelweissUtils
    {
        public static bool TryGetCustomAttribute<T>(this MemberInfo memberInfo, out T attr) where T : Attribute
        {
            attr = memberInfo.GetCustomAttribute<T>();
            return attr != null;
        }

         /// <summary>
        /// Gets all the directories and zip files in a directory, in that order and converts them to plugin assets
        /// </summary>
        public static List<PluginAsset> GetPluginAssetsFromDirectory(string directory)
        {
            List<PluginAsset> assets = [];

            foreach (string assetDir in Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly))
                assets.Add(assetDir);
            foreach (string zipPath in Directory.GetFiles(directory, "*.zip", SearchOption.TopDirectoryOnly))
                assets.Add(zipPath);

            return assets;
        }

        /// <summary>
        /// Exposes the private stream of a given zip archive
        /// </summary>
        public static FileStream Stream(this ZipArchive zipArchive)
        {
            var field = typeof(ZipArchive).GetField("_archiveStream", BindingFlags.NonPublic | BindingFlags.Instance);
            return (FileStream)field?.GetValue(zipArchive);
        }

        /// <summary>
        /// Returns the path to a given zip archive
        /// </summary>
        public static string Path(this ZipArchive zipArchive) => zipArchive.Stream().Name;
    }
}