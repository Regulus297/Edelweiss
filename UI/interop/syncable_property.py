import re

from .interop import Interop
from .py_event import PyEvent


class SyncableProperty:
    def __init__(self, prop, sync=True):
        split = prop.split(".")
        self.transform = lambda x : x.Value
        self.sync = True
        if len(split) == 2:
            self.syncable, self.prop = split
            self.syncable = Interop.getSyncable(self.syncable)
            self.leaf = True
            self._parse_prop()
        else:
            self.syncable = SyncableProperty(".".join(split[:-1]))
            if sync:
                self.syncable.ValueChanged += self._rebind_events
            self.prop = split[-1]
            self.leaf = False
            self._parse_prop()
        self._value = None if sync else self.raw_get()
        self.sync = sync
        self._events = {}
        self._init_events()

    def _init_events(self):
        var = self.raw_get()
        for prop in dir(var):
            value = getattr(var, prop)
            if type(value).__name__ == "EventBinding":
                event = PyEvent(value)
                setattr(self, prop, event)
                self._events[prop] = event

    def _rebind_events(self, _):
        var = self.raw_get()
        for name, event in self._events.items():
            event.rebind(getattr(var, name))
        self._events["ValueChanged"].invoke(self.transform(var))

    def raw_get(self):
        if self.sync:
            return getattr(self.syncable if self.leaf else self.syncable.get(), self.prop)
        return self._value

    def get(self):
        return self.transform(self.raw_get())

    def set(self, value):
        if self.leaf:
            getattr(self.syncable, self.prop).Value = value
        else:
            getattr(self.syncable.get(), self.prop).Value = value

    def _parse_prop(self):
        index_match = re.match("(.+)\\[([0-9]+)]", self.prop)
        if index_match:
            self.prop = index_match.groups()[0]
            index = int(index_match.groups()[1])
            self.raw_get().ItemChanged += lambda i, item: self._refresh_item(i, item, index)
            self.transform = lambda x: x.Value[index]

    def _refresh_item(self, i, item, index):
        if i == index:
            self._rebind_events(item)