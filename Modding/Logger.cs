using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Edelweiss.Plugins
{
    /// <summary>
    /// A class that logs messages
    /// </summary>
    /// <param name="plugin">The plugin this logger belongs to</param>
    public class Logger(Plugin plugin)
    {
        Plugin plugin = plugin;
        static ReaderWriterLockSlim readWriteLock = new ReaderWriterLockSlim();
        string filePath = null;

        static Logger()
        {
            File.Open("log.txt", FileMode.Create).Close();
        }

        /// <summary>
        /// Creates a Logger object that writes to a specific file
        /// </summary>
        /// <param name="plugin">The plugin this logger belongs to</param>
        /// <param name="filePath">The file that the logger writes to</param>
        public Logger(Plugin plugin, string filePath) : this(plugin)
        {
            this.filePath = filePath;
            File.Open(filePath, FileMode.Create).Close();
        }

        private void Write(string type, object message) => Write(plugin.ID, type, message, filePath);

        private static void Write(string id, string type, object message, string filePath = null)
        {
            WriteThreadSafe($"({DateTime.Now}) [{id}] [{type}] {message}", filePath);
        }

        private static void WriteThreadSafe(string text, string filePath = null)
        {
            readWriteLock.EnterWriteLock();
            try
            {
                using StreamWriter logWriter = new(filePath ?? "log.txt", true);
                logWriter.WriteLine(text);
                logWriter.Close();
            }
            finally
            {
                readWriteLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Outputs an empty line in the log
        /// </summary>
        public static void Break(string filePath = null) => WriteThreadSafe("", filePath);

        /// <summary>
        /// Outputs an empty line in the log
        /// </summary>
        public void Break() => Break(filePath);

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

        /// <summary>
        /// Logs a message with a given ID
        /// </summary>
        public static void Log(string id, object message) => Write(id, "Log", message);
        /// <summary>
        /// Logs a message for debugging with a given ID
        /// </summary>
        public static void Debug(string id, object message) => Write(id, "Debug", message);
        /// <summary>
        /// Logs a warning with a given ID
        /// </summary>
        public static void Warn(string id, object message) => Write(id, "Warn", message);
        /// <summary>
        /// Logs an error with a given ID
        /// </summary>
        public static void Error(string id, object message) => Write(id, "Error", message);
    }
}