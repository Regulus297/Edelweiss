using Edelweiss.Plugins;

namespace Edelweiss.Diagnostics
{
    public class EnableDiagnosticsPref : PluginSaveablePreference
    {
        public override void SetDefaultValue()
        {
            Value = true;
        }
    }
}