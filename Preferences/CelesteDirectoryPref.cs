using System;
using Edelweiss.Interop;
using Edelweiss.Plugins;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Preferences
{
    public class CelesteDirectoryPref : PluginSaveablePreference, ISyncable
    {
        public event Action OnFailedFileLoad;

        public string StringValue
        {
            get => Value?.ToString(); 
            set {
                Value = value;
            }
        }

        public override JToken Value { 
            get => base.Value; 
            set {
                base.Value = value;
                CelesteDirectory.Value = value.ToString();
            }
        }

        public BindableVariable<string> CelesteDirectory = new BindableVariable<string>();

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