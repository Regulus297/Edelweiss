using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Edelweiss.Preferences;
using Edelweiss.RegistryTypes;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss;

/// <summary>
/// 
/// </summary>
public class Main
{
    /// <summary>
    /// 
    /// </summary>
    public static void Initialize()
    {
        var watch = Stopwatch.StartNew();
        Netcode.Initialize();
        HookManager.LoadMethods();
        PluginSaveablePreference.LoadPrefs();
        PluginLoader.LoadPlugins();
        Task.Run(CelesteModLoader.LoadMods);
        watch.Stop();
        Logger.Log("Edelweiss", $"Finished loading plugins and mods in {watch.ElapsedMilliseconds} ms.");
    }

    /// <summary>
    /// 
    /// </summary>
    public static void Update()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public static void Exit()
    {
        PluginSaveablePreference.SavePrefs();
    }
}