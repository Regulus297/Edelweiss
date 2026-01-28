using Edelweiss.MVC;

namespace Edelweiss.Plugins
{
    public class PythonPlugin(string path)
    {
        [ModelProperty] public string Path { get; private set; } = path;
    }
}