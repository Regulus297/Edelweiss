using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Enumeration;
using System.Linq;
using Edelweiss.Plugins;

namespace Edelweiss.Utils
{
    /// <summary>
    /// An object that can contain either a zip file or a directory
    /// </summary>
    public class PluginAsset
    {
        /// <summary>
        /// Whether or not this asset is a zip file
        /// </summary>
        public bool IsZipFile { get; set; }

        /// <summary>
        /// The path to the directory this asset contains if it is not a zip file
        /// </summary>
        public string AssetPath { get; set; }

        /// <summary>
        /// The archive this asset contains if it is a zip file
        /// </summary>
        public ZipArchive PluginArchive { get; set; }

        private PluginAsset(bool isZipFile, string assetPath, ZipArchive pluginArchive)
        {
            IsZipFile = isZipFile;
            AssetPath = assetPath;
            PluginArchive = pluginArchive;
        }

        /// <summary>
        /// 
        /// </summary>
        ~PluginAsset()
        {
            PluginArchive?.Dispose();
        }

        /// <summary>
        /// Converts a path to a PluginAsset. If the path is a file, it is treated as a zip. If the path is a directory, the asset is treated as a directory.
        /// </summary>
        /// <param name="assetPath"></param>
        public static implicit operator PluginAsset(string assetPath) => Directory.Exists(assetPath) ? new PluginAsset(false, assetPath, null) : ZipFile.OpenRead(assetPath);

        /// <summary>
        /// Converts a zip archive into a plugin asset
        /// </summary>
        /// <param name="pluginArchive"></param>
        public static implicit operator PluginAsset(ZipArchive pluginArchive) => new PluginAsset(true, null, pluginArchive);

        /// <summary>
        /// Returns true if the asset contains a directory with the given path
        /// </summary>
        public bool DirExists(string path)
        {
            path = path.Replace(Path.DirectorySeparatorChar, '/');
            if (path == "/")
                return true;
            if (!IsZipFile)
            {
                return Directory.Exists(Path.Join(AssetPath, path));
            }
            return PluginArchive.Entries.Any(t => t.FullName.StartsWith(path));
        }

        /// <summary>
        /// Returns true if the asset contains a file with the given path
        /// </summary>
        public bool FileExists(string path)
        {
            path = path.Replace(Path.DirectorySeparatorChar, '/');
            if (path == "/")
                return true;
            if (!IsZipFile)
            {
                return File.Exists(Path.Join(AssetPath, path));
            }
            return PluginArchive.GetEntry(path) != null;
        }

        /// <summary>
        /// Returns all the files in the asset matching a given pattern
        /// </summary>
        public string[] GetFiles(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            if (!IsZipFile)
            {
                return Directory.GetFiles(AssetPath, searchPattern, searchOption);
            }
            List<string> found = [];
            foreach (ZipArchiveEntry entry in PluginArchive.Entries)
            {
                if (!FileSystemName.MatchesSimpleExpression(searchPattern, entry.Name, true))
                {
                    continue;
                }
                if (searchOption == SearchOption.AllDirectories || !entry.Name.Contains('/'))
                {
                    found.Add(entry.Name);
                }
            }
            return found.ToArray();
        }

        /// <summary>
        /// Returns all the files in the asset matching a given pattern and that are in the given path
        /// </summary>
        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            path = path.Replace(Path.DirectorySeparatorChar, '/');
            if (!IsZipFile)
            {
                return Directory.GetFiles(Path.Join(AssetPath, path), searchPattern, searchOption).Select(t => t.Substring(AssetPath.Length + 1)).ToArray();
            }
            List<string> found = [];
            foreach (ZipArchiveEntry entry in PluginArchive.Entries)
            {
                if (!FileSystemName.MatchesSimpleExpression(searchPattern, entry.Name, true) || !entry.FullName.StartsWith(path))
                {
                    continue;
                }
                if (searchOption == SearchOption.AllDirectories || !entry.FullName.Substring(path.Length + 1).Contains('/'))
                {
                    found.Add(entry.FullName);
                }
            }
            return found.ToArray();
        }

        /// <summary>
        /// Opens the entry with the given path and returns the stream
        /// </summary>
        public Stream GetStream(string path)
        {
            path = path.Replace(Path.DirectorySeparatorChar, '/');
            if (!IsZipFile && File.Exists(Path.Join(AssetPath, path)))
            {
                return File.Open(Path.Join(AssetPath, path), FileMode.Open);
            }
            return PluginArchive?.GetEntry(path)?.Open();
        }

        /// <summary>
        /// Returns the directory if the asset is a directory and the path to the zip archive if it is a zip file.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return IsZipFile ? PluginArchive.Path() : AssetPath;
        }
    }
}