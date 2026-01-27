using System;
using System.Collections.Generic;

namespace Edelweiss.MVC
{
    public class Syncable
    {
        public static event Action<string, Syncable> OnSync;
        public static readonly Dictionary<string, Syncable> syncables = [];
        public virtual void Sync(string name)
        {
            syncables[name] = this;
            OnSync?.Invoke(name, this);
        }   
    }
}