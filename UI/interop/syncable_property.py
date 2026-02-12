import re

from .interop import Interop
from .parser import NodeParser
from .py_event import PyEvent


# TODO: make type checks more robust
class SyncableProperty:
    def __init__(self, prop, sync=True, **subscribers):
        self.node = NodeParser.parse(prop, sync)
        self.prop = prop

        if self.sync and self.is_list:
            self.node.ValueChanged += self._list_changed
        elif self.sync and self.is_dict:
            self.node.ValueChanged += self._dict_changed

        for event, callbacks in subscribers.items():
            evt = getattr(self, event)
            if isinstance(callbacks, list):
                for callback in callbacks:
                    evt += callback
            else:
                evt += callbacks

        self.resync()

    def resync(self):
        self.node.ValueChanged.invoke(self.node.get())

    def _list_changed(self, _):
        for item in self.node.get():
            self.node.ItemAdded.invoke(item)

    def _dict_changed(self, _):
        for item in self.node.get():
            self.node.ItemAdded.invoke(item.Key, item.Value)

    @property
    def is_list(self):
        return self.node.type_name().startswith("BindableList[")

    @property
    def is_dict(self):
        return self.node.type_name().startswith("BindableDictionary[")

    @property
    def iterable(self):
        return self.is_list or self.is_dict

    def get(self):
        return self.node.get()

    def set(self, value):
        self.node.set(value)

    def __getattr__(self, item):
        event = self.node.get_event(item)
        if event:
            return event

        raise AttributeError(f"Property {self} has no attribute {item}")

    @property
    def sync(self):
        return self.node.sync