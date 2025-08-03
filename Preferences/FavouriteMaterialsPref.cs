using Edelweiss.Mapping.Tools;
using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Preferences
{
    internal class FavouriteMaterialsPref : PluginSaveablePreference
    {
        internal static FavouriteMaterialsPref Instance => Registry.registry[typeof(PluginSaveablePreference)].GetValue<FavouriteMaterialsPref>();
        internal static JObject Favourites => (JObject)Instance.Value;
        public override void SetDefaultValue()
        {
            Value = new JObject();
        }

        public override void PrepForSave()
        {
            JObject temp = new JObject();
            Registry.ForAll<MappingTool>(tool =>
            {
                temp.Add(tool.FullName, tool.SaveFavourites());
            });
            Value = temp;
        }
    }
}