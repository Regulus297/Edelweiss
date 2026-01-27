using System;
using Edelweiss.MVC;
using Edelweiss.RegistryTypes;

namespace Edelweiss.Plugins
{
    public class TabModelData
    {
        [ModelProperty] public event Action<Model> TabRegistered;

        private string currentTabID;
        [ModelProperty("Tab")] public string CurrentTabID { 
            get => currentTabID; 
            set
            {
                currentTabID = value;
                Console.WriteLine($"Switched Tab to: {currentTabID}");
            } 
        }
        public CustomTab CurrentTab => CustomTab.registeredTabs[CurrentTabID];

        public void RegisterTab(CustomTab tab)
        {
            TabRegistered?.Invoke(Model.Create(tab));
        }
    }
}