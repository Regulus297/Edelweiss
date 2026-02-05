using Edelweiss.Interop;
using Edelweiss.Plugins;

namespace Edelweiss
{
    public class MainVars : Loadable, ISyncable
    {
        string ISyncable.Name() => MainPlugin.Instance.ID;
        public static readonly BindableList<CustomTab> Tabs = [];
        public static readonly BindableDictionary<string, string> PythonPlugins = [];
        public static readonly BindableList<string> SampleList = ["Sample1", "Sample2"];
    }
}