from .binding_node import BindingNode


class DereferenceNode(BindingNode):
    def __init__(self, left: BindingNode, prop: str, sync = True):
        self.left = left
        self.prop = prop
        self.return_value = False
        self.return_value = self.type_name().startswith("BindableVariable[")
        super().__init__(sync)
        if self.sync:
            self.left.ValueChanged += self.rebind_events

    def get(self) -> object:
        if self.return_value:
            return getattr(self.left.get(), self.prop).Value
        return getattr(self.left.get(), self.prop)

    def set(self, value):
        getattr(self.left.get(), self.prop).Value = value

    def init_events(self):
        var = getattr(self.left.get(), self.prop)
        for prop in dir(var):
            value = getattr(var, prop)
            if type(value).__name__ == "EventBinding":
                self.add_event(prop, value)

        if "ValueChanged" not in self._events:
            self.add_event("ValueChanged", None)

    def rebind_events(self, _):
        var = getattr(self.left.get(), self.prop)
        for name, event in self._events.items():
            event.rebind(getattr(var, name))
        self._events["ValueChanged"].invoke(self.get())
