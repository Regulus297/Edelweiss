using System;

namespace Edelweiss.Interop
{
    public class BindableVariable<T>(T value = default)
    {
        protected T _value = value;
        public bool suppressed = false;

        public BindableVariable() : this(default)
        {
        }

        public event Action<T> ValueChanged;

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                if(!suppressed)
                    ValueChanged?.Invoke(_value);
            }
        }

        public SuppressBindable<T> Suppress()
        {
            return new SuppressBindable<T>(this);
        }

        public static implicit operator BindableVariable<T>(T value) => new(value);

        public override string ToString() => Value.ToString();
    }
}