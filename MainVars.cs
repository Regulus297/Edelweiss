using Edelweiss.Interop;
using Edelweiss.Plugins;

namespace Edelweiss
{
    public class MainVars : Loadable, ISyncable
    {
        string ISyncable.Name() => "Edelweiss";
        public static readonly BindableList<CustomTab> Tabs = [];
        public static readonly BindableDictionary<string, string> PythonPlugins = [];
    }
}