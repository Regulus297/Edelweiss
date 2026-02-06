from .interop import Interop
from .py_event import PyEvent


# I have no idea if this works with more than three chained accesses, please dont make me test it
class SyncableProperty:
    def __init__(self, prop, sync=True):
        split = prop.split(".")
        if len(split) == 2:
            self.syncable, self.prop = split
            self.syncable = Interop.getSyncable(self.syncable)
            self.leaf = True
        else:
            self.syncable = SyncableProperty(".".join(split[:-1]))
            self.syncable.ValueChanged += self._rebind_events
            self.prop = split[-1]
            self.leaf = False
        self.sync = True
        self._value = None if sync else self.get()
        self.sync = sync
        self._events = {}
        self._init_events()

    def _init_events(self):
        var = self.get()
        for prop in dir(var):
            value = getattr(var, prop)
            if type(value).__name__ == "EventBinding":
                event = PyEvent(value)
                setattr(self, prop, event)
                self._events[prop] = event

    def _rebind_events(self, _):
        var = self.get()
        for name, event in self._events.items():
            event.rebind(getattr(var, name))
        self._events["ValueChanged"].invoke(var.Value)

    def get(self):
        if self.sync:
            return getattr(self.syncable if self.leaf else self.syncable.get().Value, self.prop)
        return self._value

    def set(self, value):
        getattr(self.syncable, self.prop).Value = value