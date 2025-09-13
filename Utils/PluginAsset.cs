using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Enumeration;
using System.Linq;
using Edelweiss.Plugins;

namespace Edelweiss.Utils
{
    public class PluginAsset
    {
        public bool IsZipFile { get; set; }
        public string AssetPath { get; set; }
        public ZipArchive PluginArchive { get; set; }

        private PluginAsset(bool isZipFile, string assetPath, ZipArchive pluginArchive)
        {
            IsZipFile = isZipFile;
            AssetPath = assetPath;
            PluginArchive = pluginArchive;
        }

        ~PluginAsset()
        {
            PluginArchive?.Dispose();
        }

        public static implicit operator PluginAsset(string assetPath) => Directory.Exists(assetPath) ? new PluginAsset(false, assetPath, null) : ZipFile.OpenRead(assetPath);
        public static implicit operator PluginAsset(ZipArchive pluginArchive) => new PluginAsset(true, null, pluginArchive);

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

        public Stream GetStream(string path)
        {
            path = path.Replace(Path.DirectorySeparatorChar, '/');
            if (!IsZipFile && File.Exists(Path.Join(AssetPath, path)))
            {
                return File.Open(Path.Join(AssetPath, path), FileMode.Open);
            }
            return PluginArchive.GetEntry(path)?.Open();
        }

        public override string ToString()
        {
            return IsZipFile ? PluginArchive.Name() : AssetPath;
        }
    }
}