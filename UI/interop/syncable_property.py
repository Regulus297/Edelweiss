from utils import CSUtils
from .parser import NodeParser
from utils import UI


class SyncableProperty:
    def __init__(self, prop, sync=True, **subscribers):
        self.node = NodeParser.parse(prop, sync)
        self.prop = prop
        self._discarded = False

        UI.all_props.append(self)

        self._clear = None
        self._copy = False

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
                    evt += self._wrapper(callback)
            else:
                evt += self._wrapper(callbacks)

        if self.sync:
            self.resync()

    def _wrapper(self, callback):
        def inner(*args):
            try:
                callback(*args)
            except Exception as e:
                e.args = (f"{e.args[0]}\n\nIn syncable property {self.prop} of index {UI.all_props.index(self)}",)
                UI.debug()
                raise

        return inner

    def resync(self):
        self.node.ValueChanged.invoke(self.node.get())

    def _list_changed(self, _):
        if self._clear is not None:
            self._clear()

        value = self.node.get()
        if self._copy:
            value = [item for item in value]
        for item in value:
            self.node.ItemAdded.invoke(item)

    def _dict_changed(self, _):
        if self._clear is not None:
            self._clear()

        value = self.node.get()
        if self._copy:
            value = [item for item in value]
        for item in value:
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
    def copy(self):
        return self._copy

    @copy.setter
    def copy(self, value):
        if not self.iterable:
            raise TypeError(f"Cannot set copy property for non-iterable syncable property: {self.prop}")
        self._copy = value

    @property
    def is_list(self):
        return CSUtils.typeIsBindableList(type(self.node.get()))

    @property
    def is_dict(self):
        return CSUtils.typeIsBindableDict(type(self.node.get()))

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
        self._discarded = True
        del self

    def __repr__(self):
        return self.prop

    def log(self, f):
        f.write(self.prop)
        f.write(f" Discarded: {self._discarded}\n")
        for name, event in self.node._events.items():
            f.write(f"\t-{name}: {list(event._subscribers.keys())}\n")