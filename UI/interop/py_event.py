class PyEvent:
    def __init__(self, cs_event):
        self.cs_event = cs_event
        self._subscribers = []

    def __iadd__(self, callback):
        if self.cs_event is not None:
            self.cs_event += callback
        self._subscribers.append(callback)
        return self

    def __isub__(self, callback):
        if self.cs_event is not None:
            self.cs_event -= callback
        self._subscribers.remove(callback)
        return self

    def rebind(self, new_event):
        if new_event is None:
            return

        self.cs_event = new_event
        for callback in self._subscribers:
            self.cs_event += callback

    def invoke(self, *args):
        for callback in self._subscribers:
            callback(*args)