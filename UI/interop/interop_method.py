from .interop import Interop


class InteropMethod:
    def __init__(self, method):
        self.interop, self.method = method.split(".")
        self.interop = Interop.getInterop(self.interop)

    def __call__(self, *args):
        getattr(self.interop, self.method)(*args)