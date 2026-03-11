using System.Text.Json;
using Edelweiss.Interop;
using Edelweiss.Plugins;
using Newtonsoft.Json;

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
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
        {
            Converters = { new BindableConverter() }
        };
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