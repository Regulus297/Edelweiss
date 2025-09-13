using System;
using System.Diagnostics;
using Edelweiss.Plugins;

namespace Edelweiss.Diagnostics
{
    /// <summary>
    /// A disposable class that logs the time from its creation to its disposing.
    /// </summary>
    public class CustomStopwatch : IDisposable
    {
        string logMessage;
        string logID;
        Action<long> OnCompleted;
        Stopwatch stopwatch;

        /// <param name="logMessage">The message to log. Must accept one format parameter. If this is null, nothing is logged.</param>
        /// <param name="logID">The ID to log with. If null, the ID used is "EdelweissStopwatch"</param>
        /// <param name="OnCompleted">The action to perform after the stopwatch expires</param>
        public CustomStopwatch(string logMessage, string logID = null, Action<long> OnCompleted = null)
        {
            stopwatch = Stopwatch.StartNew();
            this.logMessage = logMessage;
            this.logID = logID;
            this.OnCompleted = OnCompleted;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            stopwatch.Stop();
            if (logMessage != null)
                Logger.Log(logID ?? "EdelweissStopwatch", string.Format(logMessage, stopwatch.ElapsedMilliseconds));
            OnCompleted?.Invoke(stopwatch.ElapsedMilliseconds);
        }
    }
}