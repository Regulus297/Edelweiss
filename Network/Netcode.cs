namespace Edelweiss.Network
{
    public static class Netcode
    {
        public const ulong NONE = 0x0;
        public const ulong QUIT = 0x1;
        public const ulong REGISTER_PYTHON_PLUGINS = 0x10;
        public const ulong REGISTER_SCENE = 0x11;
        public const ulong ADD_ITEM = 0x100;
        public const ulong ADD_SHAPE = 0x101;
    }
}