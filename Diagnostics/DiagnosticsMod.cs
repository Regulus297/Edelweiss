using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;
using Edelweiss.Utils;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace Edelweiss.Diagnostics
{
    internal class DiagnosticsPlugin : Plugin
    {
        public static bool Enabled => (bool)Registry.registry[typeof(PluginSaveablePreference)].GetValue<EnableDiagnosticsPref>().Value;
        public override string ID => "Diagnostics";
        Dictionary<string, long> loadTimes = [];
        public override void Load()
        {
            Logger = new Logger(this, "diagnostics.txt");
            HookManager.AddHook("CelesteModLoader.LoadMod", (Action<PluginAsset> orig, PluginAsset asset) =>
            {
                if (!Enabled)
                {
                    orig(asset);
                    return;
                }
                using (new CustomStopwatch(null, null, ms =>
                {
                    Logger.Log($"Loaded mod {asset} in {ms} ms.");
                    Logger.Break();
                    loadTimes[asset.ToString().Split('/')[^1]] = ms;
                }))
                {
                    orig(asset);
                }
            });

            HookManager.AddHook("CelesteModLoader.LoadMods", (Action orig) =>
            {
                if (!Enabled)
                {
                    orig();
                    return;
                }

                using (new CustomStopwatch(null, null, ms =>
                {
                    Logger.Break();
                    Logger.Break();
                    Logger.Log($"Finished loading mods in {ms} ms");
                    Logger.Log($"Mods arranged by descending order of load times:");
                    List<string> mods = loadTimes.Keys.ToList();
                    mods.Sort((string x, string y) => (int)loadTimes[y] - (int)loadTimes[x]);
                    foreach (string mod in mods)
                    {
                        Logger.Log($"{mod}: {loadTimes[mod]} ms");
                    }
                }))
                {
                    orig();
                }
            });
        }
    }
}