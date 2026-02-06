using Edelweiss.Interop;
using Edelweiss.Plugins;
using Edelweiss.Preferences;
using Edelweiss.RegistryTypes;

namespace Edelweiss
{
    public class MainInterop : PluginInterop
    {
        private CustomTab current = null;
        public void SetCelesteDirectory(string directory)
        {
            Registry.registry[typeof(PluginSaveablePreference)].GetValue<CelesteDirectoryPref>().StringValue = directory;
        }

        public void ChangeTab(CustomTab tab)
        {
            current?.OnDeselect();
            current = tab;
            current.Select();
        }

        public void Debug(object message)
        {
            MainPlugin.Instance.Logger.Debug(message);
        }

        public void ChangeModelName(string text)
        {
            MainVars.Model.Value.Name.Value = text;
        }

        public void ChangeModelObject()
        {
            MainVars.Model.Value = new SampleModel();
        }
    }
}