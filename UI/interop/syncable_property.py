from .interop import Interop


class SyncableProperty:
    def __init__(self, prop):
        self.syncable, self.prop = prop.split(".")
        self.syncable = Interop.getSyncable(self.syncable)

    def get(self):
        return getattr(self.syncable, self.prop)

    def set(self, value):
        setattr(self.syncable, self.prop, value)