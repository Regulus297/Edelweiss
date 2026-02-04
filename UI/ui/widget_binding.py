from interop import SyncableProperty, VariableBinding


class WidgetBinding:
    def __init__(self, data, prop, widget_setter, syncable_setter=None, custom_constructor=None):
        self.bindable, self.prop = WidgetBinding.get_property(data, prop)
        self.widget_setter = widget_setter
        self.syncable_setter = syncable_setter
        if custom_constructor is None:
            custom_constructor = VariableBinding
        self.custom_constructor = custom_constructor
        if not self.bindable:
            self.widget_setter(self.prop)
        else:
            self.custom_constructor(self.prop, self.widget_setter, self.syncable_setter)

    def bind(self, widget, signal, callback=None):
        if not self.bindable:
            return

        if callback is None:
            signal.connect(self.prop.set)
        else:
            signal.connect(lambda: callback(self.prop))

        if hasattr(widget, "__bindings__"):
            widget.__bindings__.append(self)
        else:
            widget.__bindings__ = [self]

    @staticmethod
    def get_property(data, name):
        if name in data:
            return False, data[name]
        if "bind" in data:
            if name in data["bind"]:
                prop = data["bind"][name]
                if isinstance(prop, str):
                    return True, SyncableProperty(prop)
                elif isinstance(prop, dict):
                    sync = prop.get("sync", True)
                    return True, SyncableProperty(prop["property"], sync)
        return None, None