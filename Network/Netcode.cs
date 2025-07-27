namespace Edelweiss.Network
{
    public static class Netcode
    {
        public const long NONE = 0x0;
        public const long QUIT = 0x1;
        public const long REGISTER_PYTHON_PLUGINS = 0x2;
        public const long REGISTER_SCENE = 0x3;
        public const long REGISTER_JSON_SCENE = 0x4;
        public const long ADD_ITEM = 0x5;
        public const long ADD_SHAPE = 0x6;


        // Received packets
        public const long BUTTON_PRESSED = -0x1;
        public const long LIST_SELECTION_CHANGED = -0x2;
    }
}