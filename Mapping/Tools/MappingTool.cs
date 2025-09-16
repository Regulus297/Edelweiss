using System;
using System.Collections.Generic;
using System.Linq;
using Edelweiss.Plugins;
using Edelweiss.Preferences;
using Edelweiss.RegistryTypes;
using Edelweiss.Utils;
using Newtonsoft.Json.Linq;

namespace Edelweiss.Mapping.Tools
{
    /// <summary>
    /// The base class for any tools used in mapping
    /// </summary>
    [BaseRegistryObject()]
    public abstract class MappingTool : PluginRegistryObject
    {
        internal string selectedMaterial = "";
        internal int selectedLayer = 0;
        internal int selectedMode = 0;

        /// <summary>
        /// The display name of the tool.
        /// </summary>
        public string DisplayName => Plugin.GetLocalization($"Tools.{Name}");

        /// <summary>
        /// Returns true if the input material matches the current search term
        /// </summary>
        protected bool IsSearched(string material) => material == null ? false : MappingTab.searchTerm == "" || material.Contains(MappingTab.searchTerm, StringComparison.CurrentCultureIgnoreCase);

        /// <summary>
        /// If true, clicking will also trigger <see cref="MouseDrag"/> as well as <see cref="MouseClick"/>  
        /// </summary>
        public virtual bool ClickingTriggersDrag => false;

        /// <inheritdoc/>
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

        /// <summary>
        /// Called when the mouse is clicked while this tool is selected
        /// </summary>
        /// <param name="room">The current selected room. Null if no room is selected</param>
        /// <param name="x">The x-coordinate of the mouse</param>
        /// <param name="y">The y-coordinate of the mouse</param>
        public virtual void MouseClick(JObject room, float x, float y)
        {

        }


        /// <summary>
        /// Called when the mouse is dragged while this tool is selected
        /// </summary>
        /// <param name="room">The current selected room. Null if no room is selected</param>
        /// <param name="x">The x-coordinate of the mouse</param>
        /// <param name="y">The y-coordinate of the mouse</param>
        public virtual void MouseDrag(JObject room, float x, float y)
        {

        }

        /// <summary>
        /// Called when the mouse is released while this tool is selected
        /// </summary>
        /// <param name="room">The current selected room. Null if no room is selected</param>
        /// <param name="x">The x-coordinate of the mouse</param>
        /// <param name="y">The y-coordinate of the mouse</param>
        public virtual void MouseRelease(JObject room, float x, float y)
        {

        }

        /// <summary>
        /// Called when a particular material is favourited by the user
        /// </summary>
        /// <param name="material">The ID of the favourited material</param>
        public virtual void OnFavourited(string material)
        {

        }

        /// <summary>
        /// Loads the favourites from the saved preference
        /// </summary>
        /// <param name="data">The favourites for this particular tool</param>
        public virtual void LoadFavourites(JObject data)
        {

        }

        /// <summary>
        /// Saves the favourited materials as a JToken
        /// </summary>
        public virtual JToken SaveFavourites()
        {
            return new JObject();
        }

        /// <summary>
        /// Called when the tool is selected
        /// </summary>
        public virtual void OnSelect()
        {

        }

        /// <summary>
        /// Called when the tool is deselected
        /// </summary>
        public virtual void OnDeselect()
        {

        }

        /// <summary>
        /// Gets the materials that the tool should display.
        /// </summary>
        /// <returns>A dictionary of the material IDs to the display names</returns>
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

        /// <summary>
        /// The material names of the tool
        /// </summary>
        public List<string> Materials => GetMaterials().Values.ToList();

        /// <summary>
        /// The material IDs of the tool
        /// </summary>
        public List<string> MaterialIDs => GetMaterials().Keys.ToList();

        /// <summary>
        /// The layers the tool can operate on. Usually affects which materials are available
        /// </summary>
        public virtual List<string> Layers => [];

        internal List<string> LayerNames => Layers.Select(Plugin.GetLocalization).ToList();

        /// <summary>
        /// The modes the tool can have.
        /// </summary>
        public virtual List<string> Modes => [];

        internal List<string> ModeNames => Modes.Select(Plugin.GetLocalization).ToList();
    }
}