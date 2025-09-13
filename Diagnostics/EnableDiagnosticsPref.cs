using Edelweiss.Plugins;

namespace Edelweiss.Diagnostics
{
    internal class EnableDiagnosticsPref : PluginSaveablePreference
    {
        public override void SetDefaultValue()
        {
            Value = true;
        }
    }
}