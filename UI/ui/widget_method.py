from interop import InteropMethod


class WidgetMethod:
    def __init__(self, data, signal, parameters, binding):
        self.interop_method = InteropMethod(data["method"])
        self.args = data["args"]
        self.parameters = parameters
        self.binding = binding
        signal.connect(self._invoke)

    def _invoke(self, *params):
        args = []
        for arg in self.args:
            if isinstance(arg, str) and arg.startswith("@"):
                if arg.startswith("@@"):
                    arg = arg[1:]
                else:
                    param = arg[1:]
                    if param.isnumeric():
                        args.append(params[int(param)])
                    else:
                        args.append(self.parameters[param]())
                    continue
            args.append(arg)

        if self.binding is not None:
            self.binding.suppress = True
        self.interop_method(*args)
        if self.binding is not None:
            self.binding.suppress = False

    @classmethod
    def create(cls, widget, signal, data, name, binding, parameters=None):
        if name not in data:
            return None

        if parameters is None:
            parameters = {}

        method = cls(data[name], signal, parameters, binding)
        setattr(widget, f"__method_{name}__", method)
        return method