using Edelweiss.Interop;
using Edelweiss.Plugins;

namespace Edelweiss
{
    public class MainVars : Loadable, ISyncable
    {
        string ISyncable.Name() => MainPlugin.Instance.ID;
        public static readonly BindableList<CustomTab> Tabs = [];
        public static readonly BindableDictionary<string, string> PythonPlugins = [];
        public static readonly BindableVariable<SampleModel> Model = new SampleModel();
    }

    public class SampleModel
    {
        public BindableVariable<string> Name = "";
        public BindableVariable<string> Directory = "";
        public BindableList<string> SampleList = ["Sample"];
    }
}