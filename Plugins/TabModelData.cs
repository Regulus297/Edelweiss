using System;
using Edelweiss.MVC;
using Edelweiss.MVC.Controllers;
using Edelweiss.RegistryTypes;

namespace Edelweiss.Plugins
{
    [CustomController(typeof(TabModelDataController))]
    public class TabModelData
    {
        [ModelProperty] public event Action<Model> TabRegistered;
        [ModelProperty] public event Action<Model> SwitchedTab;

        private string currentTabID;
        [ModelProperty("Tab")] public string CurrentTabID { 
            get => currentTabID; 
            set
            {
                currentTabID = value;
                SwitchedTab?.Invoke(Model.Create(CurrentTab));
            } 
        }
        
        [ModelProperty("TabObject")] public CustomTab CurrentTab => CustomTab.registeredTabs[CurrentTabID];

        public void RegisterTab(CustomTab tab)
        {
            TabRegistered?.Invoke(Model.Create(tab));
        }
    }
}