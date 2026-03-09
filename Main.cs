using Edelweiss.Plugins;

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
        PluginSaveablePreference.LoadPrefs();
        PluginLoader.LoadPlugins();
    }

    /// <summary>
    /// 
    /// </summary>
    public static void Exit()
    {
        PluginSaveablePreference.SavePrefs();
    }
}