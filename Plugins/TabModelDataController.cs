using System;
using Edelweiss.MVC.Controllers;

namespace Edelweiss.Plugins
{
    public class TabModelDataController : Controller
    {
        public void ToolbarAction(string action)
        {
            ((CustomTab)Model.Get("TabObject")).HandleToolbarClick(action);
        }
        public void SetField(string fieldName, object value)
        {
            Model.Set(fieldName, value);
        }
    }
}