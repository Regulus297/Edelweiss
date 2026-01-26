using System;
using Edelweiss.Interop;
using Edelweiss.Plugins;

namespace Edelweiss.Preferences
{
    public class CelesteDirectoryPref : PluginSaveablePreference, ISyncable
    {
        public event Action OnFailedFileLoad;

        public string StringValue
        {
            get => Value?.ToString(); 
            set => Value = value;
        }

        public override void SetDefaultValue()
        {
            OnFailedFileLoad?.Invoke();
        }

        string ISyncable.Name()
        {
            return FullName;
        }
    }
}