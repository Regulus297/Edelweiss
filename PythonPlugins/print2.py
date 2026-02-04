from plugins import plugin_loadable, load_dependencies


@plugin_loadable
@load_dependencies("Edelweiss:print")
class Print2:
    def __init__(self):
        Print("Hello2")