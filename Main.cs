using System;
using Edelweiss.Network;
using Edelweiss.Plugins;

namespace Edelweiss;

public class Main
{
    static int times = 0;
    public static void Initialize()
    {
        PluginSaveablePreference.LoadPrefs();
        PluginLoader.LoadPlugins();
    }
    public static void Update()
    {
        times++;
        if (times > 9E5)
        {
            NetworkManager.SendPacket(Netcode.QUIT, "");
        }
    }

    public static void Exit()
    {
        PluginSaveablePreference.SavePrefs();
    }
}