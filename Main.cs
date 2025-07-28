using System;
using System.Collections.Generic;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss;

public class Main
{
    public static void Initialize()
    {
        Netcode.Initialize();
        PluginSaveablePreference.LoadPrefs();
        PluginLoader.LoadPlugins();
    }
    public static void Update()
    {
    }

    public static void Exit()
    {
        PluginSaveablePreference.SavePrefs();
    }
}