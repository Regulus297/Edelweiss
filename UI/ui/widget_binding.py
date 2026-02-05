from interop import SyncableProperty, VariableBinding


class WidgetBinding:
    def __init__(self, data, prop, widget_setter, syncable_setter=None, custom_constructor=None):
        self.bindable, self.prop = WidgetBinding.get_property(data, prop)
        if self.prop is None:
            return
        self.widget_setter = widget_setter
        self.syncable_setter = syncable_setter
        self.signal_updates = {}
        if custom_constructor is None:
            custom_constructor = VariableBinding
        self.custom_constructor = custom_constructor
        if not self.bindable:
            self.widget_setter(self.prop)
        else:
            self.binding = self.custom_constructor(self.prop, self.widget_setter, self.syncable_setter)

    def bind(self, widget, signal, callback=None):
        if not self.bindable:
            return

        self.signal_updates[signal] = False
        signal.connect(self._callback_wrapper(callback))

        if hasattr(widget, "__bindings__"):
            widget.__bindings__.append(self)
        else:
            widget.__bindings__ = [self]

    def _callback_wrapper(self, callback):
        def inner():
            self.binding.suppress = True
            callback(self.binding)
            self.binding.suppress = False
        return inner

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

    @staticmethod
    def get_value(data, name):
        bindable, prop = WidgetBinding.get_property(data, name)
        if bindable:
            return prop.get().Value
        return prop