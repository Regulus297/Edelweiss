using System;

namespace Edelweiss.Interop
{
    public class BindableVariable<T>(T value = default)
    {
        private T _value = value;

        public event Action<T> ValueChanged;

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                ValueChanged?.Invoke(_value);
            }
        }

        public static implicit operator BindableVariable<T>(T value) => new(value);

        public override string ToString() => Value.ToString();
    }
}