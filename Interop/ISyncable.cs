using System;
using System.Collections.Generic;

namespace Edelweiss.Interop
{
    public interface ISyncable
    {
        public readonly static Dictionary<string, ISyncable> Syncables = [];
        public string Name();
    }
}