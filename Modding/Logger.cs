using System;
using System.IO;

namespace Edelweiss.Plugins
{
    public class Logger(Plugin plugin)
    {
        Plugin plugin = plugin;
        static StreamWriter logWriter = new StreamWriter(File.Open("log.txt", FileMode.Create));
        private void Write(string type, object message)
        {
            logWriter.WriteLine($"({DateTime.Now}) [{plugin.ID}] [{type}] {message}");
        }

        public void Log(object message) => Write("Log", message);
        public void Debug(object message) => Write("Debug", message);
        public void Warn(object message) => Write("Warn", message);
        public void Error(object message) => Write("Error", message);

        internal static void Close()
        {
            logWriter.Close();
        }
    }
}