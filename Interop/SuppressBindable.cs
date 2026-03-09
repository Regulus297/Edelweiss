using System;

namespace Edelweiss.Interop
{
    public class SuppressBindable<T> : IDisposable
    {
        private BindableVariable<T> Variable;

        public SuppressBindable(BindableVariable<T> variable)
        {
            Variable = variable;
            Variable.suppressed = true;
        }

        public void Dispose()
        {
            Variable.suppressed = false;
        }
    }
}