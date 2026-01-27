using System;
using Edelweiss.Plugins;

namespace Edelweiss;

public class Main
{
    public static void Initialize()
    {
        PluginSaveablePreference.LoadPrefs();
        PluginLoader.LoadPlugins();
    }

    public static void PostLoad()
    {
        PluginLoader.PostLoad();
    }

    public static void Exit()
    {
        PluginSaveablePreference.SavePrefs();
    }
}