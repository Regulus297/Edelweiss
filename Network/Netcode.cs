namespace Edelweiss.Network
{
    public static class Netcode
    {
        public const long NONE = 0x0;
        public const long QUIT = 0x1;
        public const long REGISTER_PYTHON_PLUGINS = 0x10;
        public const long REGISTER_SCENE = 0x11;
        public const long REGISTER_JSON_SCENE = 0x100;
        public const long ADD_ITEM = 0x101;
        public const long ADD_SHAPE = 0x110;


        // Received packets
        public const long BUTTON_PRESSED = -0x1;
    }
}