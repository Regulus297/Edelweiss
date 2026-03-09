from .parser import NodeParser


# TODO: make type checks more robust
class SyncableProperty:
    def __init__(self, prop, sync=True, **subscribers):
        self.node = NodeParser.parse(prop, sync)
        self.prop = prop

        self._clear = None

        if self.sync and self.is_list:
            self.node.ValueChanged += self._list_changed
        elif self.sync and self.is_dict:
            self.node.ValueChanged += self._dict_changed

        self.add_subscribers(**subscribers)

    def add_subscribers(self, **subscribers):
        for event, callbacks in subscribers.items():
            evt = getattr(self, event)
            if isinstance(callbacks, list):
                for callback in callbacks:
                    evt += callback
            else:
                evt += callbacks

        if self.sync:
            self.resync()

    def resync(self):
        self.node.ValueChanged.invoke(self.node.get())

    def _list_changed(self, _):
        if self._clear is not None:
            self._clear()
        for item in self.node.get():
            self.node.ItemAdded.invoke(item)

    def _dict_changed(self, _):
        if self._clear is not None:
            self._clear()
        for item in self.node.get():
            self.node.ItemAdded.invoke(item.Key, item.Value)

    @property
    def clear(self):
        return self._clear

    @clear.setter
    def clear(self, value):
        if not self.iterable:
            raise TypeError(f"Cannot set clear method for non-iterable syncable property: {self.prop}")
        self._clear = value

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

    def add(self, *args):
        self.node.get().Add(*args)

    def remove(self, value):
        self.node.get().Remove(value)

    def set_item(self, key, value):
        self.node.get()[key] = value

    def __getattr__(self, item):
        event = self.node.get_event(item)
        if event:
            return event

        raise AttributeError(f"Property {self.prop} has no attribute {item}")

    @property
    def sync(self):
        return self.node.sync

    def discard(self):
        self.node.discard()
        del self