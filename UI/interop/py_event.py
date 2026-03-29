import traceback


class PyEvent:
    def __init__(self, cs_event):
        self.cs_event = cs_event
        self._subscribers = {}
        self.blocked = False
        self.before = []
        self.after = []
        self._queue = []
        if self.cs_event is not None:
            self.cs_event += self.invoke

    def __iadd__(self, callback):
        self._subscribers[callback] = "\n".join(traceback.format_stack()[:-1])
        return self

    def __isub__(self, callback):
        del self._subscribers[callback]
        return self

    def rebind(self, new_event):
        if new_event is None:
            return

        self.cs_event = new_event
        self.cs_event += self.invoke

    def invoke(self, *args):
        if self.blocked:
            return

        for callback in self.before:
            callback()

        for callback in self._subscribers:
            try:
                callback(*args)
            except Exception as e:
                e.args = (f"{e.args[0]}\n\nIn event invocation in callback registered at: {self._subscribers[callback]}",)
                raise

        for callback in self.after:
            callback()

        for callback in self._queue:
            callback()
        self._queue.clear()

    def setBlocked(self, blocked):
        self.blocked = blocked

    def pair(self, other):
        other.before.append(lambda: self.setBlocked(True))
        other.after.append(lambda: self.setBlocked(False))
        self.before.append(lambda: other.setBlocked(True))
        self.after.append(lambda: other.setBlocked(False))

    def queue(self, callback):
        self._queue.append(callback)

    def discard(self):
        self._subscribers.clear()
