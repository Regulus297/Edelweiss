using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        Netcode.Initialize();
        PluginSaveablePreference.LoadPrefs();
        PluginLoader.LoadPlugins();
        CelesteModLoader.LoadMods();
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