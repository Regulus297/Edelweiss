using System;
using Edelweiss.MVC;
using Edelweiss.Plugins;

namespace Edelweiss.Preferences
{
    [CreateModelOnLoad]
    public class CelesteDirectoryPref : PluginSaveablePreference
    {
        [ModelProperty] public event Action OnFailedFileLoad;
        [ModelProperty("Value")] public string StringValue
        {
            get => Value?.ToString(); 
            set => Value = value;
        }

        public override void SetDefaultValue()
        {
            OnFailedFileLoad?.Invoke();
        }
    }
}