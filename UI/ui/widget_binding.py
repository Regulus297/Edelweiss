from interop import SyncableProperty
from interop.py_event import PyEvent


class WidgetBinding:
    _signal_lookup = {}

    def __init__(self, data, name, **subscribers):
        self.bindable, self.prop = WidgetBinding.get_property(data, name, **subscribers)
        if not self.bindable:
            if "ValueChanged" in subscribers:
                callbacks = subscribers["ValueChanged"]
                if isinstance(callbacks, list):
                    for callback in callbacks:
                        callback(self.prop)
                else:
                    callbacks(self.prop)
            elif "ItemAdded" in subscribers:
                callbacks = subscribers["ItemAdded"]
                for item in self.prop:
                    if isinstance(callbacks, list):
                        for callback in callbacks:
                            callback(item)
                    else:
                        callbacks(item)


    @staticmethod
    def get_property(data, name, **subscribers):
        if name in data:
            return False, data[name]
        if "bind" in data and name in data["bind"]:
            return True, SyncableProperty(data["bind"][name], True, **subscribers)
        return None, None

    @staticmethod
    def get_value(data, name):
        bindable, prop = WidgetBinding.get_property(data, name)
        if bindable:
            return prop.get()
        return prop

    @staticmethod
    def bind(signal, *callbacks, pair=None, call_args=None):
        event = WidgetBinding._signal_lookup.get(signal)
        if event is None:
            event = PyEvent(None)
            signal.connect(event.invoke)
            WidgetBinding._signal_lookup[signal] = event

        if pair is not None:
            event.pair(pair)

        for callback in callbacks:
            if call_args is not None:
                callback(*call_args)
            event += callback

    @property
    def is_dict(self):
        if self.bindable:
            return self.prop.is_dict
        return isinstance(self.prop, dict)

    @property
    def is_list(self):
        if self.bindable:
            return self.prop.is_list
        return isinstance(self.prop, list)