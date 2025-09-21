using System;
using System.Collections.Generic;
using System.IO;
using Edelweiss.Network;
using Edelweiss.Preferences;
using Edelweiss.RegistryTypes;
using Edelweiss.Utils;

namespace Edelweiss.Plugins
{
    /// <summary>
    /// The basic plugin class
    /// </summary>
    [BaseRegistryObject()]
    public abstract class Plugin : IRegistryObject
    {
        /// <summary>
        /// The ID of the plugin. Every plugin needs to have a unique identifier.
        /// </summary>
        public virtual string ID => GetType().Name;
        internal event Action OnPostSetupContent;
        /// <summary>
        /// The logger instance associated with this plugin
        /// </summary>
        public Logger Logger;

        /// <summary>
        /// 
        /// </summary>
        public void OnRegister()
        {
            Logger = new(this);
            Load();
        }

        /// <summary>
        /// Called when the plugin is loaded
        /// </summary>
        public virtual void Load()
        {

        }

        internal void PostLoad()
        {
            PostSetupContent();
            OnPostSetupContent.Invoke();
        }

        /// <summary>
        /// Called after all plugins have been loaded
        /// </summary>
        public virtual void PostSetupContent()
        {

        }

        /// <summary>
        /// Creates a netcode with a given identifier prefixed by the plugin's ID.
        /// </summary>
        /// <param name="name">The name of the netcode</param>
        /// <param name="positive">If true, the generated netcode will be positive.</param>
        public long CreateNetcode(string name, bool positive) => Netcode.CreateNetcode($"{ID}:{name}", positive);

        /// <summary>
        /// Prefixes the plugin's ID to the given key.
        /// </summary>
        public string GetLocalizationKey(string key) => $"{ID}.{key}";

        /// <summary>
        /// Prefixes the plugin's ID to the given key and attempts to get the localization for it.
        /// </summary>
        public string GetLocalization(string key)
        {
            return Language.GetText(GetLocalizationKey(key));
        }

        /// <summary>
        /// Creates a cache file with the given filename at the cache directory for the plugin
        /// </summary>
        /// <param name="filename">The filename (with extension) for the cache file</param>
        /// <returns>The stream of the cache file</returns>
        public Stream CreateCache(string filename)
        {
            string cacheDirectory = Path.Join(Directory.GetCurrentDirectory(), ".cache", ID);
            if (!Directory.Exists(cacheDirectory))
            {
                Directory.CreateDirectory(cacheDirectory);
            }

            return File.OpenWrite(Path.Join(cacheDirectory, filename));
        }

        /// <summary>
        /// Returns whether or not the given cache exists.
        /// </summary>
        /// <param name="filename">The cache path</param>
        public bool CacheExists(string filename)
        {
            return File.Exists(CachePath(filename));
        }

        /// <summary>
        /// Returns the absolute path to the cache with the given name
        /// </summary>
        public string CachePath(string filename)
        {
            return Path.Join(Directory.GetCurrentDirectory(), ".cache", ID, filename);
        }
    }
}