from .interop import Interop


class SyncableProperty:
    def __init__(self, prop, sync=True):
        self.syncable, self.prop = prop.split(".")
        self.syncable = Interop.getSyncable(self.syncable)
        self.sync = True
        self._value = None if sync else self.get()
        self.sync = sync

    def get(self):
        if self.sync:
            return getattr(self.syncable, self.prop)
        return self._value

    def set(self, value):
        getattr(self.syncable, self.prop).Value = value