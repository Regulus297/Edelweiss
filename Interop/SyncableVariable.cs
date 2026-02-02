using System;

namespace Edelweiss.Interop
{
    public class SyncableVariable<T> : ISyncable
    {
        public string Name;
        public event Action<T> OnUpdate;
        private T value;

        public SyncableVariable(string name, T value)
        {
            Name = name;
            this.value = value;
            this.Sync();
        }

        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                OnUpdate?.Invoke(this.value);
            }
        }

        public void ForceUpdate()
        {
            OnUpdate?.Invoke(value);
        }

        string ISyncable.Name() => Name;
    }
}