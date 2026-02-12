from ..py_event import PyEvent


class BindingNode:
    def __init__(self, sync):
        self.sync = sync
        self._events = {}
        if sync:
            self.init_events()

    def get(self) -> object:
        ...

    def set(self, value):
        ...

    def init_events(self):
        var = self.get()
        for prop in dir(var):
            value = getattr(var, prop)
            if type(value).__name__ == "EventBinding":
                self.add_event(prop, value)

        if "ValueChanged" not in self._events:
            self.add_event("ValueChanged", None)

    def rebind_events(self, _):
        var = self.get()
        for name, event in self._events.items():
            event.rebind(getattr(var, name, None))
        self._events["ValueChanged"].invoke(var)

    def type_name(self):
        return type(self.get()).__name__

    def add_event(self, name, cs_event):
        event = PyEvent(cs_event)
        setattr(self, name, event)
        self._events[name] = event

    def get_event(self, name):
        return self._events.get(name)