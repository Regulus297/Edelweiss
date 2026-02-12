from .binding_node import BindingNode
from ..interop import Interop


class SyncableNode(BindingNode):
    def __init__(self, syncable):
        self.syncable = Interop.getSyncable(syncable)
        super().__init__(True)

    def get(self) -> object:
        return self.syncable

    def set(self, value):
        raise RuntimeError("Cannot set value of SyncableNode")