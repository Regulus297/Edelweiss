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
        PluginSaveablePreference.LoadPrefs();
        PluginLoader.LoadPlugins();

        // JObject rect = new()
        // {
        //     {"name", "rect1"},
        //     {"x", 0},
        //     {"y", 0},
        //     {"width", 90},
        //     {"height", 80},
        //     {"shapes", JToken.FromObject(new List<JObject>() {
        //         new JObject() {
        //             {"type", "rectangle"},
        //             {"color", "#ffffff"},
        //             {"fill", "#ff0000"},
        //             { "thickness", 5f},
        //             {"x", 0},
        //             {"y", 0},
        //             {"width", 1.0},
        //             {"height", 1.0}
        //         }
        //     })}
        // };
        // NetworkManager.SendPacket(Netcode.ADD_ITEM, rect);

        // JObject rect2 = new()
        // {
        //     {"name", "rect1"},
        //     {"shape",
        //         new JObject() {
        //             {"type", "rectangle"},
        //             {"color", "#ffffff"},
        //             { "thickness", 5f},
        //             {"x", 0},
        //             {"y", 0},
        //             {"width", 0.5},
        //             {"height", 1.0}
        //         }
        //     }
        // };
        // NetworkManager.SendPacket(Netcode.ADD_SHAPE, rect2);
    }
    public static void Update()
    {
    }

    public static void Exit()
    {
        PluginSaveablePreference.SavePrefs();
    }
}