def plugin_loadable(cls):
    cls.__is_loadable__ = True
    return cls