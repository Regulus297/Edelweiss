using System;
using Edelweiss.Plugins;

namespace Edelweiss.Preferences
{
    internal class CelesteDirectoryPref : PluginSaveablePreference
    {
        public override void SetDefaultValue()
        {
            Console.WriteLine("Enter Celeste Directory: ");
            Value = Console.ReadLine();
        }
    }
}