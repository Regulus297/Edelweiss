from .dereference_node import DereferenceNode
from .binding_node import BindingNode


class IndexerNode(BindingNode):
    def __init__(self, left: BindingNode, index, sync = True):
        self.left = left
        self.index = index

        super().__init__(sync)
        if self.sync:
            self.left.ValueChanged += self.rebind_events
            self.left.ItemChanged += self._refresh_value
            self.left.ItemRemoved += self._item_removed

    def get(self) -> object:
        return self.left.get()[self.index]

    def set(self, value):
        self.left.get()[self.index] = value

    def _refresh_value(self, i, value):
        if i == self.index:
            self.ValueChanged.invoke(value)

    def _item_removed(self, i, _):
        if i < self.index:
            self.index -= 1