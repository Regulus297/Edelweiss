using System;
using Edelweiss.Plugins;

namespace Edelweiss.Preferences
{
    public class CelesteDirectoryPref : PluginSaveablePreference
    {
        public override void SetDefaultValue()
        {
            Console.WriteLine("Enter Celeste Directory: ");
            Value = Console.ReadLine();
        }
    }
}