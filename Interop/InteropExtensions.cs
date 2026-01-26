using System;

namespace Edelweiss.Interop
{
    public static class InteropExtensions
    {
        public static event Action<string, object> OnSync;
        public static void Sync(this ISyncable self)
        {
            ISyncable.Syncables[self.Name()] = self;
            OnSync?.Invoke(self.Name(), self);
        }
    }
}