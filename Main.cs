using System;
using System.Collections.Generic;
using Edelweiss.Network;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss;

public class Main
{
    static int times = 0;
    public static void Initialize()
    {
        PluginSaveablePreference.LoadPrefs();
        PluginLoader.LoadPlugins();

        JObject rect = new()
        {
            {"x", 0},
            {"y", 0},
            {"width", 90},
            {"height", 80},
            {"shapes", JToken.FromObject(new List<JObject>() {
                new JObject() {
                    {"type", "rectangle"},
                    {"color", "#ffffff"},
                    {"width", 5f}
                }
            })}
        };
        NetworkManager.SendPacket(Netcode.ADD_ITEM, rect);
        Console.WriteLine(rect.ToString());
    }
    public static void Update()
    {
    }

    public static void Exit()
    {
        PluginSaveablePreference.SavePrefs();
    }
}