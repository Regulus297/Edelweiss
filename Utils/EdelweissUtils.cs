using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace Edelweiss.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class EdelweissUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The type of attribute to get</typeparam>
        /// <param name="memberInfo"></param>
        /// <param name="attr"></param>
        /// <returns>Whether the member has the attribute</returns>
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

        // Copied from https://www.extensionmethod.net/csharp/type/issubclassofrawgeneric
        /// <summary>
        /// Alternative version of <see cref="Type.IsSubclassOf"/> that supports raw generic types (generic types without
        /// any type parameters).
        /// </summary>
        /// <param name="baseType">The base type class for which the check is made.</param>
        /// <param name="toCheck">To type to determine for whether it derives from <paramref name="baseType"/>.</param>
        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type baseType)
        {
            while (toCheck != typeof(object))
            {
                Type cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (baseType == cur)
                {
                    return true;
                }

                toCheck = toCheck.BaseType;
            }

            return false;
        }
    }
}