using System.Collections.Generic;
using System.Linq;
using Edelweiss.Plugins;
using Edelweiss.Preferences;
using Edelweiss.RegistryTypes;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Tools
{
    [BaseRegistryObject()]
    public abstract class MappingTool : PluginRegistryObject
    {
        internal string selectedMaterial = "";
        internal int selectedLayer = 0;
        internal int selectedMode = 0;
        public virtual string DisplayName => Name.Substring(0, Name.Length - 4).CamelCaseToText();

        public virtual bool ClickingTriggersDrag => false;
        public override void PostSetupContent()
        {
            if (FavouriteMaterialsPref.Favourites.ContainsKey(FullName))
            {
                LoadFavourites(FavouriteMaterialsPref.Favourites.Value<JObject>(FullName));
            }
            else
            {
                FavouriteMaterialsPref.Favourites.Add(FullName, new JObject());
            }
        }
        internal void MouseDown(JObject room, float x, float y)
        {
            if (ClickingTriggersDrag)
                MouseDrag(room, x, y);
            MouseClick(room, x, y);
        }

        public virtual void MouseClick(JObject room, float x, float y)
        {

        }

        public virtual void MouseDrag(JObject room, float x, float y)
        {

        }

        public virtual void MouseRelease(JObject room, float x, float y)
        {

        }

        public virtual void OnFavourited(string material)
        {
            
        }

        public virtual void LoadFavourites(JObject data)
        {
            
        }

        public virtual JToken SaveFavourites()
        {
            return new JObject();
        }

        public virtual Dictionary<string, string> GetMaterials()
        {
            return [];
        }

        /// <summary>
        /// Defines special behaviour for the cursor ghost when the current tool is selected.
        /// Returns false by default
        /// </summary>
        /// <param name="mouseX">The x-coordinate of the mouse in scene coordinates</param>
        /// <param name="mouseY">The y-coordinate of the mouse in scene coordinates</param>
        /// <returns>False if default update logic should be used, true to suppress default logic</returns>
        public virtual bool UpdateCursorGhost(float mouseX, float mouseY)
        {
            return false;
        }

        public List<string> Materials => GetMaterials().Values.ToList();
        public List<string> MaterialIDs => GetMaterials().Keys.ToList();
        public virtual List<string> Layers => [];
        public virtual List<string> Modes => [];
    }
}