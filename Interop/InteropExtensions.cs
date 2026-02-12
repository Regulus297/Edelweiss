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

        public static void SafeInvoke<T>(this T evt, params object[] args) where T: MulticastDelegate
        {
            var handlers = evt;
            foreach(T h in handlers.GetInvocationList())
            {
                
            }
        }
    }
}