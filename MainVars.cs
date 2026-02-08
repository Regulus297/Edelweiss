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
        public BindableVariable<string> Selectable = "";
        public BindableVariable<string> Plugin = "";
        public BindableVariable<bool> Bool = true;
        public BindableVariable<int> Int = 12;
        public override string ToString() => $"{Name} {Directory} {Selectable} {Plugin} {Bool} {Int}";
    }
}