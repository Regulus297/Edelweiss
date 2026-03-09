using System;

namespace Edelweiss.Interop
{
    /// <summary>
    /// Disposable object that suppresses event invocations for its bindable
    /// </summary>
    public class SuppressBindable<T> : IDisposable
    {
        private BindableVariable<T> Variable;

        /// <summary>
        /// 
        /// </summary>
        public SuppressBindable(BindableVariable<T> variable)
        {
            Variable = variable;
            Variable.suppressed = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Variable.suppressed = false;
        }
    }
}