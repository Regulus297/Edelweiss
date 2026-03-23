from interop import SyncableProperty
from interop.py_event import PyEvent


class WidgetBinding:
    _signal_lookup = {}

    def __init__(self, widget, data, name, **subscribers):
        if hasattr(widget, "__bindings__"):
            widget.__bindings__.append(self)
        else:
            widget.__bindings__ = [self]

        self.bindable, self.prop = WidgetBinding.get_property(data, name)
        if self.bindable:
            self.prop.add_subscribers(**subscribers)
        elif self.bindable is not None:
            self._sync_non_bindable(**subscribers)

    def _sync_non_bindable(self, **subscribers):
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
    def get_property(data, name):
        if name in data:
            return False, data[name]
        if "bind" in data and name in data["bind"]:
            return True, SyncableProperty(data["bind"][name], True)
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

    def discard(self):
        if self.bindable:
            self.prop.discard()