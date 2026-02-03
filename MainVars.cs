using Edelweiss.Interop;
using Edelweiss.Plugins;

namespace Edelweiss
{
    public class MainVars : Loadable, ISyncable
    {
        string ISyncable.Name() => "Edelweiss";
        public static BindableList<CustomTab> Tabs = [];
    }
}