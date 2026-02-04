from plugins import plugin_loadable


@plugin_loadable
class Print:
    def __init__(self, value="Hello"):
        print(value)