using System;
using System.IO;

namespace Edelweiss.Plugins
{
    /// <summary>
    /// A class that logs messages
    /// </summary>
    /// <param name="plugin">The plugin this logger belongs to</param>
    public class Logger(Plugin plugin)
    {
        Plugin plugin = plugin;

        static Logger()
        {
            File.Open("log.txt", FileMode.Create).Close();
        }
        private void Write(string type, object message)
        {
            using StreamWriter logWriter = new("log.txt", true);
            logWriter.WriteLine($"({DateTime.Now}) [{plugin.ID}] [{type}] {message}");
        }

        /// <summary>
        /// Logs a message
        /// </summary>
        public void Log(object message) => Write("Log", message);

        /// <summary>
        /// Logs a message for debugging
        /// </summary>
        public void Debug(object message) => Write("Debug", message);

        /// <summary>
        /// Logs a warning
        /// </summary>
        public void Warn(object message) => Write("Warn", message);

        /// <summary>
        /// Logs an error
        /// </summary>
        public void Error(object message) => Write("Error", message);

    }
}