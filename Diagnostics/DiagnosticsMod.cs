using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;

namespace Edelweiss.Diagnostics
{
    public class DiagnosticsPlugin : Plugin
    {
        public static bool Enabled => (bool)Registry.registry[typeof(PluginSaveablePreference)].GetValue<EnableDiagnosticsPref>().Value;
        public override string ID => "Diagnostics";
        public override void Load()
        {
        }
    }
}