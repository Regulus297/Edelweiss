using System;

namespace Edelweiss.Interop
{
    /// <summary>
    /// A variable that invokes an event when its value is changed
    /// </summary>
    /// <typeparam name="T">The type this variable contains</typeparam>
    public class BindableVariable<T>(T value = default)
    {
        /// <summary>
        /// 
        /// </summary>
        protected T _value = value;

        /// <summary>
        /// Whether this variable is blocked from invoking events
        /// </summary>
        public bool suppressed = false;

        /// <summary>
        /// 
        /// </summary>
        public BindableVariable() : this(default)
        {
        }

        /// <summary>
        /// Invoked when the value of this variable is set
        /// </summary>
        public event Action<T> ValueChanged;

        /// <summary>
        /// The value of the variable. Setting it will invoke ValueChanged
        /// </summary>
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

        /// <summary>
        /// Returns an object to suppress event invocations of this variable until disposed
        /// </summary>
        public SuppressBindable<T> Suppress()
        {
            return new SuppressBindable<T>(this);
        }

        /// <summary>
        /// 
        /// </summary>
        public static implicit operator BindableVariable<T>(T value) => new(value);

        /// <inheritdoc/>
        public override string ToString() => Value.ToString();
    }
}