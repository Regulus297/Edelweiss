from interop import InteropMethod


class WidgetMethod:
    def __init__(self, data, signal, parameters):
        self.interop_method = InteropMethod(data["method"])
        self.args = data["args"]
        self.parameters = parameters
        signal.connect(self._invoke)

    def _invoke(self):
        args = []
        for arg in self.args:
            if isinstance(arg, str) and arg.startswith("@"):
                if arg.startswith("@@"):
                    arg = arg[1:]
                else:
                    param = arg[1:]
                    args.append(self.parameters[param]())
                    continue
            args.append(arg)

        self.interop_method(*args)

    @classmethod
    def create(cls, widget, signal, data, name, parameters=None):
        if name not in data:
            return None

        if parameters is None:
            parameters = {}

        method = cls(data[name], signal, parameters)
        setattr(widget, f"__method_{name}__", method)
        return method