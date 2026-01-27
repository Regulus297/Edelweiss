using System;
using Edelweiss.Plugins;
using Edelweiss.RegistryTypes;

namespace Edelweiss.MVC
{
    public class CreateModelOnLoadAttribute(LoadStage loadStage = LoadStage.OnLoad) : PluginLoadAttribute
    {
        public readonly LoadStage LoadStage = loadStage;

        public override void OnLoad(IRegistryObject value)
        {
            if(LoadStage == LoadStage.OnLoad)
                Model.Create(value);
        }

        public override void PostLoadTypes(IRegistryObject value)
        {
            if(LoadStage == LoadStage.PostLoadTypes)
                Model.Create(value);
        }

        public override void PostLoadPlugins(IRegistryObject value)
        {
            if(LoadStage == LoadStage.PostLoadPlugins)
                Model.Create(value);
        }
        
        public override void PostLoadUI(IRegistryObject value)
        {
            if(LoadStage == LoadStage.PostLoadUI)
                Model.Create(value);
        }
    }
}